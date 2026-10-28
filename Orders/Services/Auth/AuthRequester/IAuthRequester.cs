using Orders.Dtos;

namespace Orders.Services.Auth.AuthRequester
{
    public interface IAuthRequester
    {
        Task<AuthCardentials> GetAccessToken(CancellationToken ct = default);
    }
}
