using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public class RefundNotAllowedFabric : ExceptionsFabric
    {
        protected override BaseCreationException CreateException(string type)
        {
            throw new RefundNotAllowed("Возврат по данному платежу невозможен", type);
        }
    }
}
