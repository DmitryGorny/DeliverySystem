using Catalog.Common.Exceptions.ExceptionsDtos;
using Catalog.Dtos.enums;

namespace Catalog.Common.Exceptions
{
    public class ReservationException<T> : Exception where T : BaseDetails
    {
        public List<T> Details { get; private set; }
        public FailureType FailureType { get; private set; }
        public ReservationException(string message, List<T> details, FailureType type) : base(message)
        {
            Details = details;
            FailureType = type;
        }
    }
}
