using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public class NotFoundFabric : ExceptionsFabric
    {
        protected override BaseCreationException CreateException(string type)
        {
            throw new NotFoundException("Платеж не найден", type);
        }
    }
}
