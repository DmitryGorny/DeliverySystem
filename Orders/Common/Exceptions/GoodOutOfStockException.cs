using Orders.Common.Exceptions.ExcpetionsDtos;

namespace Orders.Common.Exceptions
{
    public class GoodOutOfStockException : Exception
    {
        public List<OutOfStockDetails> Goods { get; private set; }
        public GoodOutOfStockException(
            string message,
            List<OutOfStockDetails> details) : base(message)
        {
            Goods = details;
        }
    }
}
