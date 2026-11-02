using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public class ValidationFabric : ExceptionsFabric
    {
        protected override BaseCreationException CreateException(string type)
        {
            throw new ValidationException("Данные по заказу невалидны", type);
        }
    }
}
