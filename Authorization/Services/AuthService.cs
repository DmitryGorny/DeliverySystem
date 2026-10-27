using Authorization.Dtos;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Authorization.Services
{
    public class AuthService : IAuthService
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            IOpenIddictApplicationManager applicationManager,
            IConfiguration configuration
            )
        {
            _applicationManager = applicationManager;
            _configuration = configuration;
        }

        public async Task<ClaimsPrincipal> AuthorizeService(OpenIddictRequest request)
        {
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("The application cannot be found.");

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

            identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

            identity.SetDestinations(static claim => [Destinations.AccessToken]);

            return new ClaimsPrincipal(identity);
        }
        
        public async Task SeedServicesCardentials(CancellationToken ct)
        {
            var servicesCardentials = _configuration         
                                        .GetSection("ServicesCardentials")
                                        .Get<List<WebServiceCardentials>>() ?? [];

           foreach (var cardentials in servicesCardentials)
            {
                if (await _applicationManager.FindByClientIdAsync(cardentials.ClientId, ct) is not null)
                    continue;

                await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = cardentials.ClientId,
                    ClientSecret = cardentials.ClientSecret,
                    Permissions = //TODO: пока общие, потом подумать над парсингом
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials
                    }
                }, ct);

            };
        }
    }
}
