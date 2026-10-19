using Orders.Common.Exceptions.ExcpetionsDtos;

namespace Orders.Common.Exceptions
{
    public class GoodOutOfStockException : Exception
    {
        private List<OutOfStockDetails> Details { get; }
        public GoodOutOfStockException(
            string message,
            List<OutOfStockDetails> details) : base(message)
        {
            Details = details;
        }
    }
}
