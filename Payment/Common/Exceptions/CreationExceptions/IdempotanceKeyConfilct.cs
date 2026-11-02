namespace Payment.Common.Exceptions.CreationExceptions
{
    public class IdempotanceKeyConfilct : BaseCreationException
    {
        public IdempotanceKeyConfilct(string message, string type) : base(message, type)
        {
        }

        public override int HttpStatus { get; protected set; } = 500;
    }
}
