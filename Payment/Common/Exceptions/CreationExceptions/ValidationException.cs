namespace Payment.Common.Exceptions.CreationExceptions
{
    public class ValidationException : BaseCreationException
    {
        public ValidationException(string message, string type) : base(message, type)
        {
        }

        public override int HttpStatus { get; protected set; } = 400;

    }
}
