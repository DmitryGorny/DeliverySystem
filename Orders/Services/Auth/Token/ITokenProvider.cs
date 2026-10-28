using Orders.Dtos;

namespace Orders.Services.Auth.Token;
public interface ITokenProvider
{
    AuthCardentials GetAuthCardentials();
    void SetAuthCardentials(AuthCardentials authCardentials);
}
