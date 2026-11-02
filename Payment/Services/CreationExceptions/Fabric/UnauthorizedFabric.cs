using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public class UnauthorizedFabric : ExceptionsFabric
    {
        protected override BaseCreationException CreateException(string type)
        {
            throw new UnauthorizedException("Внутренняя ошибка сервиса", type);
        }
    }
}
