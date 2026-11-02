namespace Payment.Common.Exceptions.CreationExceptions
{
    public class NotFoundException : BaseCreationException
    {
        public NotFoundException(string message, string type) : base(message, type)
        {
        }

        public override int HttpStatus { get; protected set; } = 404;
    }
}
