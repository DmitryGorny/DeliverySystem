namespace Orders.Common.Exceptions.PaymentExceptions
{
    public class PaymentSystemException : Exception
    {
        public PaymentSystemException(string message, int code) : base(message) 
        {
            HttpStatus = code;
        }

        public int HttpStatus { get; set; }
    }
}
