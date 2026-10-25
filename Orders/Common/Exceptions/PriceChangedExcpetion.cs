using Orders.Common.Exceptions.ExcpetionsDtos;

namespace Orders.Common.Exceptions
{
    public class PriceChangedExcpetion : Exception
    {
        public List<PriceChangedDetails> Details { get; private set; }
        public PriceChangedExcpetion(string message, List<PriceChangedDetails> details) : base(message)
        {
            Details = details;
        }
    }
}
