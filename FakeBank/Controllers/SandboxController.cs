using FakeBank.Infrastructure;
using FakeBank.Models;
using FakeBank.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeBank.Controllers
{
    // «Действия покупателя на странице банка». Без авторизации.
    [ApiController]
    public class SandboxController(BankService bank) : ControllerBase
    {
        // Сюда ведёт confirmationUrl
        [HttpGet("pay/{id:guid}")]
        public IResult PaymentPage(Guid id) =>
            bank.GetPayment(id) is { } payment ? Results.Ok(payment) : BankProblems.NotFound("Payment");

        [HttpPost("sandbox/payments/{id:guid}/succeed")]
        public IResult Succeed(Guid id) => ToResult(id, bank.Decide(id, success: true));

        [HttpPost("sandbox/payments/{id:guid}/cancel")]
        public IResult Cancel(Guid id, [FromQuery] string? reason = null)
        {
            var parsedReason = CancellationReason.InsufficientFunds;
            if (reason is not null && !BankJson.TryParseSnake(reason, out parsedReason))
            {
                return BankProblems.Create(StatusCodes.Status400BadRequest, "validation_error",
                    "Unknown reason. Use insufficient_funds, card_expired, fraud_suspected or canceled_by_payer");
            }

            return ToResult(id, bank.Decide(id, success: false, parsedReason));
        }

        private IResult ToResult(Guid id, DecideResult result) => result switch
        {
            DecideResult.NotFound => BankProblems.NotFound("Payment"),
            DecideResult.NotPending => BankProblems.Create(StatusCodes.Status409Conflict, "payment_not_pending",
                "Payment is already processed"),
            _ => Results.Ok(bank.GetPayment(id))
        };
    }
}
