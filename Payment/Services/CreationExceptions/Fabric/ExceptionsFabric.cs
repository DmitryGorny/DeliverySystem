using Payment.Common.Exceptions.CreationExceptions;

namespace Payment.Services.CreationExceptions.Fabric
{
    public abstract class ExceptionsFabric
    {
        protected abstract BaseCreationException CreateException(string type);

        public void Throw(string type)
        {
            throw CreateException(type);
        }
    }
}
