namespace Payment.Common.Exceptions.CreationExceptions
{
    public class UnauthorizedException : BaseCreationException
    {
        public UnauthorizedException(string message, string type) : base(message, type)
        {
        }

        public override int HttpStatus { get; protected set; } = 500;
    }
}
