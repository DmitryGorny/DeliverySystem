using FakeBank.Dtos;
using FakeBank.Infrastructure;
using FakeBank.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeBank.Controllers
{
    [ApiController]
    [Route("v1/refunds")]
    public class RefundsController(BankService bank, IdempotencyStore idempotency) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType<RefundResponse>(StatusCodes.Status201Created)]
        public IResult Create(
            [FromHeader(Name = "Idempotence-Key")] string idempotenceKey,
            CreateRefundRequest request)
        {
            RefundCreateResult result = RefundCreateResult.Created;

            var (check, response) = idempotency.Execute(
                "refunds", idempotenceKey, IdempotencyStore.Hash(request),
                () =>
                {
                    var (r, refund) = bank.CreateRefund(request);
                    result = r;
                    return (refund, r == RefundCreateResult.Created);
                });

            if (check == IdempotencyCheck.Conflict)
            {
                return BankProblems.Create(StatusCodes.Status409Conflict, "idempotence_key_conflict",
                    "Idempotence key was already used with a different request body");
            }

            if (check == IdempotencyCheck.Replay)
            {
                var replayed = (RefundResponse)response!;
                return Results.Created($"/v1/refunds/{replayed.Id}", replayed);
            }

            return result switch
            {
                RefundCreateResult.PaymentNotFound => BankProblems.NotFound("Payment"),
                RefundCreateResult.NotAllowed => BankProblems.Create(StatusCodes.Status422UnprocessableEntity,
                    "refund_not_allowed", "Refund is allowed only for succeeded payments"),
                RefundCreateResult.AmountExceeded => BankProblems.Create(StatusCodes.Status422UnprocessableEntity,
                    "refund_amount_exceeded", "Total refunded amount would exceed the payment amount"),
                _ => Results.Created($"/v1/refunds/{((RefundResponse)response!).Id}", response)
            };
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType<RefundResponse>(StatusCodes.Status200OK)]
        public IResult Get(Guid id) =>
            bank.GetRefund(id) is { } refund ? Results.Ok(refund) : BankProblems.NotFound("Refund");
    }
}
