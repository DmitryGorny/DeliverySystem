using Microsoft.AspNetCore.Diagnostics;
using Orders.Common.Exceptions.PaymentExceptions;
using System.Text.Json;

namespace Orders.Services.ExceptionHandlers
{
    public class PaymentExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is PaymentSystemException)
            {
                var castedException = (PaymentSystemException)exception;

                httpContext.Response.StatusCode = castedException.HttpStatus;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(
                    new { Message = exception.Message}
                    ));

                return true;
            }

            return false;
        }
    }
}
