namespace Payment.Common.Exceptions.CreationExceptions
{
    public class RefundNotAllowed : BaseCreationException
    {
        public RefundNotAllowed(string message, string type) : base(message, type)
        {
        }

        public override int HttpStatus { get; protected set; } = 409;
    }
}
