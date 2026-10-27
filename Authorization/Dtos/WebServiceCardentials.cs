using OpenIddict.Abstractions;

namespace Authorization.Dtos
{
    public class WebServiceCardentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

    }
}
