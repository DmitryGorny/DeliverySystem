namespace Payment.Common.Exceptions.CreationExceptions
{
    public class IdempotanceKeyException : BaseCreationException
    {
        public IdempotanceKeyException(string message, string type) : base(message, type)
        {
        }

        public override int HttpStatus { get; protected set; } = 500;
    }
}
