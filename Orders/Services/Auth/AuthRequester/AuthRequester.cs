using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Orders.Dtos;

namespace Orders.Services.Auth.AuthRequester
{
    public class AuthRequester : IAuthRequester
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;

        public AuthRequester(HttpClient http, IConfiguration config)
        {
            _http = http;
            _configuration = config;
        }
        public async Task<AuthCardentials> GetAccessToken(CancellationToken ct = default)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _configuration["AuthCardentials:ClientId"]!,
                ["client_secret"] = _configuration["AuthCardentials:ClientSecret"]!
            });


            using var request = await _http.PostAsync("api/Auth/ServicesAuth", content, ct);

            request.EnsureSuccessStatusCode();

            return await request.Content.ReadFromJsonAsync<AuthCardentials>(ct);
        }
    }
}
