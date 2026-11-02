using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public class IdempotenceConflictFabric : ExceptionsFabric
    {
        protected override BaseCreationException CreateException(string type)
        {
            return new IdempotanceKeyConfilct("Внутренняя ошибка сервиса", type);
        }
    }
}
