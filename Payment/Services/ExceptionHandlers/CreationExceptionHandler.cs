using Microsoft.AspNetCore.Diagnostics;
using Payment.Common.Exceptions.CreationExceptions;
using System.Text.Json;

namespace Payment.Services.ExceptionHandlers
{
    public class CreationExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var castedException = exception as BaseCreationException;

            if (castedException != null)
            {
                httpContext.Response.StatusCode = castedException.HttpStatus;
                await httpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new {
                            Message = castedException.Message,
                            Type = castedException.Type}
                    ), 
                    cancellationToken);
                return true;
            }
            return false;
        }
    }
}
