
using Orders.Dtos;

namespace Orders.Services.Auth.Token;
public class TokenProvider : ITokenProvider
{
    public string Token { get; set; }
    public string Type { get; set; }
    public DateTime ExpiresIn { get; set; }

    public AuthCardentials GetAuthCardentials()
    {
        if (DateTime.Now > ExpiresIn)
            throw new ArgumentException("Время жизни токена вышло");

        return new AuthCardentials
        {
            AccessToken = Token,
            ExpiresIn = ExpiresIn.Second,
            TokenType = Type
        };
    }

    public void SetAuthCardentials(AuthCardentials authCardentials)
    {
        Token = authCardentials.AccessToken;
        Type = authCardentials.TokenType;
        ExpiresIn = DateTime.UtcNow.AddSeconds(authCardentials.ExpiresIn);
    }
}
