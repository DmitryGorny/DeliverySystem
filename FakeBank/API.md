# FakeBank API

Эмулятор внешнего платёжного провайдера (по мотивам ЮKassa). Твой сервис платежей
работает с ним так же, как работал бы с настоящим банком.

Базовый адрес: `http://localhost:5100`

---

## Авторизация

Все запросы к `/v1/*` требуют заголовок:

```
Authorization: Bearer <ApiKey>
```

`ApiKey` задаётся в `appsettings.json` банка (`Merchant:ApiKey`, по умолчанию `test_api_key`).
Без заголовка или с неверным ключом → `401 Unauthorized`.

## Идемпотентность

Все `POST`-запросы к `/v1/*` требуют заголовок:

```
Idempotence-Key: <любая уникальная строка до 64 символов>
```

- Повтор запроса с **тем же ключом и тем же телом** → банк ничего не создаёт заново
  и возвращает тот же ответ, что и в первый раз.
- Тот же ключ с **другим телом** → `409 Conflict`.
- Без заголовка → `400 Bad Request`.

Удобно использовать `orderId` как ключ для создания платежа: тогда ретрай после таймаута
не создаст второй платёж.

## Формат JSON

- Имена полей — camelCase.
- Enum-ы — строками (`"pending"`, `"succeeded"` …).
- Деньги — число с двумя знаками после запятой (`12990.00`).
- Время — UTC в ISO 8601 (`2026-10-26T12:00:00Z`).

## Ошибки

Все ошибки возвращаются в формате ProblemDetails (`application/problem+json`):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Idempotence key was already used with a different request body",
  "status": 409,
  "code": "idempotence_key_conflict"
}
```

| Код | `code` | Когда |
|---|---|---|
| 400 | `validation_error` | невалидное тело (сумма ≤ 0, нет обязательных полей) |
| 400 | `idempotence_key_required` | нет заголовка `Idempotence-Key` |
| 401 | `unauthorized` | нет или неверный API-ключ |
| 404 | `not_found` | платёж / возврат не найден |
| 409 | `idempotence_key_conflict` | ключ уже использован с другим телом |
| 422 | `refund_not_allowed` | возврат по платежу не в статусе `succeeded` |
| 422 | `refund_amount_exceeded` | сумма возвратов больше суммы платежа |

---

## Платежи

### Статусы платежа

```
pending ──► succeeded
   │
   └────► canceled   (с причиной cancellationReason)
```

- `pending` — платёж создан, ждём оплату от покупателя.
- `succeeded` — деньги списаны.
- `canceled` — оплата не прошла. Причина в `cancellationReason`:
  `insufficient_funds`, `card_expired`, `fraud_suspected`, `canceled_by_payer`.

Платёж может **зависнуть** в `pending` навсегда (так банк эмулирует отсутствие ответа —
см. `HangRate` в настройках). Таймаут ожидания оплаты — ответственность твоей системы.

### `POST /v1/payments` — создать платёж

Заголовки: `Authorization`, `Idempotence-Key`.

```json
{
  "amount": 12990.00,
  "currency": "RUB",
  "orderId": "7d3e1b9a-4c2f-4a8e-9b61-0f5c2d8e1a33",
  "description": "Заказ 7d3e1b9a"
}
```

| Поле | Тип | Обязательное |
|---|---|---|
| `amount` | decimal, > 0 | да |
| `currency` | `"RUB"` | нет, по умолчанию `RUB` |
| `orderId` | guid — твой идентификатор заказа, банк его просто хранит и возвращает | да |
| `description` | string, до 128 символов | нет |

Ответ `201 Created` (заголовок `Location: /v1/payments/{id}`):

```json
{
  "id": "2c1f8a40-6b7e-4d19-a3c5-9e0b4f72d811",
  "status": "pending",
  "amount": 12990.00,
  "currency": "RUB",
  "orderId": "7d3e1b9a-4c2f-4a8e-9b61-0f5c2d8e1a33",
  "description": "Заказ 7d3e1b9a",
  "confirmationUrl": "http://localhost:5100/pay/2c1f8a40-6b7e-4d19-a3c5-9e0b4f72d811",
  "cancellationReason": null,
  "refundedAmount": 0.00,
  "createdAt": "2026-10-26T12:00:00Z",
  "processedAt": null
}
```

`confirmationUrl` — ссылка, которую показывают покупателю для оплаты.

### `GET /v1/payments/{id}` — получить платёж

Ответ `200 OK` — тот же объект платежа. `404`, если не найден.

Используй для сверки статуса, если webhook не пришёл.

---

## Возвраты

### Статусы возврата

```
pending ──► succeeded
```

Возврат выполняется асинхронно, через несколько секунд (`RefundDelaySeconds`).

### `POST /v1/refunds` — создать возврат

Заголовки: `Authorization`, `Idempotence-Key`.

```json
{
  "paymentId": "2c1f8a40-6b7e-4d19-a3c5-9e0b4f72d811",
  "amount": 12990.00
}
```

- Возврат возможен только по платежу в статусе `succeeded`, иначе `422 refund_not_allowed`.
- Можно делать частичные возвраты; сумма всех возвратов не может превышать сумму платежа,
  иначе `422 refund_amount_exceeded`.

Ответ `201 Created`:

```json
{
  "id": "9a7b6c5d-1111-2222-3333-444455556666",
  "paymentId": "2c1f8a40-6b7e-4d19-a3c5-9e0b4f72d811",
  "status": "pending",
  "amount": 12990.00,
  "createdAt": "2026-10-26T12:10:00Z",
  "processedAt": null
}
```

### `GET /v1/refunds/{id}` — получить возврат

Ответ `200 OK` — объект возврата. `404`, если не найден.

---

## Webhook-уведомления (банк → твой сервис)

Когда статус платежа или возврата меняется, банк отправляет `POST` на адрес
`Merchant:WebhookUrl` из настроек банка (по умолчанию `http://localhost:5005/webhooks/bank`).

### События

| `event` | Когда | `object` |
|---|---|---|
| `payment.succeeded` | оплата прошла | объект платежа |
| `payment.canceled` | оплата не прошла | объект платежа (с `cancellationReason`) |
| `refund.succeeded` | возврат выполнен | объект возврата |

### Тело

```json
{
  "id": "e1c0b5c2-....",
  "event": "payment.succeeded",
  "createdAt": "2026-10-26T12:00:04Z",
  "object": {
    "id": "2c1f8a40-6b7e-4d19-a3c5-9e0b4f72d811",
    "status": "succeeded",
    "amount": 12990.00,
    "currency": "RUB",
    "orderId": "7d3e1b9a-4c2f-4a8e-9b61-0f5c2d8e1a33",
    "...": "..."
  }
}
```

`id` уведомления уникален: по нему можно отсеивать дубли.

### Подпись

Заголовок `X-Bank-Signature` = HMAC-SHA256 от **сырого тела запроса** (байты UTF-8)
с ключом `Merchant:WebhookSecret` (по умолчанию `test_webhook_secret`), в виде hex строчными буквами.

Проверяй подпись перед обработкой; при несовпадении отвечай `401`.

### Доставка

- Банк считает уведомление доставленным, если твой сервис ответил **любым 2xx** в течение 5 секунд.
- Иначе — повторяет: через 5 с, 15 с, 30 с, 1 мин, 2 мин, затем раз в 5 мин, всего до 20 попыток.
- Уведомления **могут приходить повторно** (например, если ты обработал, но ответ потерялся).
  Обработка обязана быть идемпотентной.
- Порядок уведомлений не гарантирован.

---

## Песочница (эмуляция покупателя)

Эти эндпоинты без авторизации — это «действия покупателя на странице банка».

### `GET /pay/{id}`

То, что открывается по `confirmationUrl`. Возвращает платёж (JSON) — просто чтобы ссылка работала.

### `POST /sandbox/payments/{id}/succeed`

Покупатель оплатил. Работает только для `pending`, иначе `409`.

### `POST /sandbox/payments/{id}/cancel?reason=insufficient_funds`

Оплата не прошла. `reason` — одна из причин выше, по умолчанию `insufficient_funds`.

В режиме `Auto` банк сам решает судьбу платежа, но ручные эндпоинты тоже работают:
можно «оплатить» вручную раньше, чем банк решит сам.

---

## Настройки банка (`appsettings.json`)

| Ключ | По умолчанию | Смысл |
|---|---|---|
| `Bank:Mode` | `Auto` | `Auto` — банк сам решает исход платежа; `Manual` — только через `/sandbox` |
| `Bank:FailureRate` | `0.1` | доля отказов в режиме `Auto` |
| `Bank:HangRate` | `0.0` | доля платежей, которые навсегда останутся `pending` (эмуляция «банк не ответил») |
| `Bank:MinDelaySeconds` / `MaxDelaySeconds` | `1` / `5` | через сколько секунд банк решает исход платежа |
| `Bank:RefundDelaySeconds` | `2` | через сколько выполняется возврат |
| `Bank:PublicBaseUrl` | `http://localhost:5100` | база для `confirmationUrl` |
| `Merchant:ApiKey` | `test_api_key` | ключ для `Authorization: Bearer` |
| `Merchant:WebhookUrl` | `http://localhost:5005/webhooks/bank` | куда слать уведомления |
| `Merchant:WebhookSecret` | `test_webhook_secret` | ключ подписи уведомлений |

Хранилище — в памяти: после перезапуска банк всё забывает (как будто это другой день в песочнице).
