using System.Text.Json;
using System.Text.Json.Serialization;

namespace FakeBank.Infrastructure
{
    public static class BankJson
    {
        // camelCase для полей, snake_case для enum-ов: "succeeded", "insufficient_funds"
        public static readonly JsonSerializerOptions Options = Create();

        public static void Apply(JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        }

        private static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            Apply(options);
            return options;
        }

        // "insufficient_funds" -> CancellationReason.InsufficientFunds
        public static bool TryParseSnake<TEnum>(string? value, out TEnum result) where TEnum : struct, Enum
        {
            result = default;
            return !string.IsNullOrWhiteSpace(value)
                && Enum.TryParse(value.Replace("_", ""), ignoreCase: true, out result)
                && Enum.IsDefined(result);
        }
    }
}
