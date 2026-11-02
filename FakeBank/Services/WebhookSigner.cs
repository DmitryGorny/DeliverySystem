using System.Security.Cryptography;
using System.Text;
using FakeBank.Options;
using Microsoft.Extensions.Options;

namespace FakeBank.Services
{
    public class WebhookSigner(IOptions<MerchantOptions> options)
    {
        public const string HeaderName = "X-Bank-Signature";

        // HMAC-SHA256 от сырого тела, hex строчными буквами
        public string Sign(byte[] body)
        {
            var key = Encoding.UTF8.GetBytes(options.Value.WebhookSecret);
            return Convert.ToHexString(HMACSHA256.HashData(key, body)).ToLowerInvariant();
        }
    }
}
