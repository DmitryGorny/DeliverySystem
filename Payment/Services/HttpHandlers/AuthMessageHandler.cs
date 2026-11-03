using System.Net.Http.Headers;

namespace Payment.Services.HttpHandler
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly IConfiguration _config;

        public AuthMessageHandler(IConfiguration conf)
        {
            _config = conf;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    _config["ApiKey:Bank"]
            );
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
