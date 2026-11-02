using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public class IdempotanceKeyFabric : ExceptionsFabric
    {
        protected override BaseCreationException CreateException(string type)
        {
            throw new IdempotanceKeyConfilct("Внутренняя ошибка сервиса", type);
        }
    }
}
