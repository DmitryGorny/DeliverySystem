namespace Payment.Common.Exceptions.CreationExceptions
{
    public class RefundAmountExceeded : BaseCreationException
    {
        public RefundAmountExceeded(string message, string type) : base(message, type)
        {
        }

        public override int HttpStatus { get; protected set; } = 409;
    }
}
