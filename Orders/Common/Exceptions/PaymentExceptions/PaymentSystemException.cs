namespace Orders.Common.Exceptions.PaymentExceptions
{
    public class PaymentSystemException : Exception
    {
        public PaymentSystemException() : base() { }
        public PaymentSystemException(string message, int code) : base(message) 
        {
            HttpStatus = code;
            Message = message;
        }

        public string Message { get; set; }

        public int HttpStatus { get; set; }
    }
}
