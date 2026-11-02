using FakeBank.Dtos;
using FakeBank.Infrastructure;
using FakeBank.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeBank.Controllers
{
    [ApiController]
    [Route("v1/payments")]
    public class PaymentsController(BankService bank, IdempotencyStore idempotency) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType<PaymentResponse>(StatusCodes.Status201Created)]
        public IResult Create(
            [FromHeader(Name = "Idempotence-Key")] string idempotenceKey,
            CreatePaymentRequest request)
        {
            var (check, response) = idempotency.Execute(
                "payments", idempotenceKey, IdempotencyStore.Hash(request),
                () => (bank.CreatePayment(request), true));

            if (check == IdempotencyCheck.Conflict)
            {
                return BankProblems.Create(StatusCodes.Status409Conflict, "idempotence_key_conflict",
                    "Idempotence key was already used with a different request body");
            }

            var payment = (PaymentResponse)response!;
            return Results.Created($"/v1/payments/{payment.Id}", payment);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType<PaymentResponse>(StatusCodes.Status200OK)]
        public IResult Get(Guid id) =>
            bank.GetPayment(id) is { } payment ? Results.Ok(payment) : BankProblems.NotFound("Payment");
    }
}
