using System.Security.Cryptography;
using System.Text.Json;

namespace FakeBank.Infrastructure
{
    public enum IdempotencyCheck
    {
        // Ключ новый — выполняем запрос
        New,
        // Тот же ключ и то же тело — отдаём сохранённый ответ
        Replay,
        // Тот же ключ, другое тело
        Conflict
    }

    public class IdempotencyStore
    {
        private record Entry(string RequestHash, object? Response);

        private readonly object _sync = new();
        private readonly Dictionary<string, Entry> _entries = new();

        public static string Hash(object request)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(request, BankJson.Options);
            return Convert.ToHexString(SHA256.HashData(bytes));
        }

        // Атомарно проверяет ключ и, если он новый, выполняет action.
        // Результат запоминается, только если action вернул Remember = true (ошибки не кэшируются).
        public (IdempotencyCheck Check, object? Response) Execute(
            string scope, string key, string requestHash, Func<(object? Response, bool Remember)> action)
        {
            var fullKey = $"{scope}:{key}";

            lock (_sync)
            {
                if (_entries.TryGetValue(fullKey, out var entry))
                {
                    return entry.RequestHash == requestHash
                        ? (IdempotencyCheck.Replay, entry.Response)
                        : (IdempotencyCheck.Conflict, null);
                }

                var (response, remember) = action();
                if (remember)
                {
                    _entries[fullKey] = new Entry(requestHash, response);
                }
                return (IdempotencyCheck.New, response);
            }
        }
    }
}
