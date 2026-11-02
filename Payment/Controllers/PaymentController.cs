using Microsoft.AspNetCore.Mvc;
using Payment.Dtos;
using Payment.Services.Payment;

namespace Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("CreatePayment")]
        public async Task<ActionResult<CreatePaymentResponse>> CreatePayment([FromBody] CreatePaymentBody body)
        {
            return await _paymentService.CreatePayment(body);
        }
    }
}
