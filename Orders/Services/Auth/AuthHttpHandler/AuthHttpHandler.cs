using Orders.Services.Auth.AuthRequester;
using Orders.Services.Auth.Token;
using System.Net.Http.Headers;

namespace Orders.Services.Auth.AuthHttpHandler
{
    public class AuthHttpHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IAuthRequester _authRequester;
        public AuthHttpHandler(ITokenProvider tokenProvider, IAuthRequester requester)
        {
            _tokenProvider = tokenProvider;
            _authRequester = requester;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var cardentials = _tokenProvider.GetAuthCardentials();

                request.Headers.Authorization = new AuthenticationHeaderValue(
                    cardentials.TokenType, 
                    cardentials.AccessToken);
            } catch (ArgumentException ex)
            {
                var cardentials = await _authRequester.GetAccessToken(cancellationToken);

                _tokenProvider.SetAuthCardentials(cardentials);

                request.Headers.Authorization = new AuthenticationHeaderValue(
                    cardentials.TokenType,
                    cardentials.AccessToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
