using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace Authorization.Services
{
    public interface IAuthService
    {
        Task<ClaimsPrincipal> AuthorizeService(OpenIddictRequest request);
        Task SeedServicesCardentials(CancellationToken ct);
    }
}
