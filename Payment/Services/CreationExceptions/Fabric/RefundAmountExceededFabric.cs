using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public class RefundAmountExceededFabric : ExceptionsFabric
    {
        protected override BaseCreationException CreateException(string type)
        {
            throw new RefundAmountExceeded("Сумма возврата превышает сумму платежа", type);
        }
    }
}
