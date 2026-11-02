using Payment.Dtos;

namespace Payment.Common.Exceptions.CreationExceptions
{
    public abstract class BaseCreationException : Exception
    {
        public BaseCreationException(string message, string type) : base(message)
        {
            Message = message;
            Type = type;
        }
        public string Type { get; set; }
        public abstract int HttpStatus { get; protected set; }
        public readonly string Message;
    }
}
