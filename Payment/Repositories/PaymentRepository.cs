using Payment.DB;
using Payment.Models;

namespace Payment.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;

        public PaymentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CreatedPayments> CreatePayment(CreatedPayments paymentCreated)
        {
            _db.Add(paymentCreated);
            await _db.SaveChangesAsync();
            return paymentCreated;
        }
    }
}
