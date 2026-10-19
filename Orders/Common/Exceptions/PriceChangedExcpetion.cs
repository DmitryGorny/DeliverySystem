using Orders.Common.Exceptions.ExcpetionsDtos;

namespace Orders.Common.Exceptions
{
    public class PriceChangedExcpetion : Exception
    {
        private List<PriceChangedDetails> Details { get; }
        public PriceChangedExcpetion(string message, List<PriceChangedDetails> details) : base(message)
        {
            Details = details;
        }
    }
}
