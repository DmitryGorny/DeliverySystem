namespace FakeBank.Options
{
    public enum BankMode
    {
        // Банк сам решает исход платежа через случайную задержку
        Auto,
        // Исход платежа задаётся только через /sandbox
        Manual
    }

    public class BankOptions
    {
        public const string SectionName = "Bank";

        public BankMode Mode { get; set; } = BankMode.Auto;
        public double FailureRate { get; set; } = 0.1;
        public double HangRate { get; set; } = 0.0;
        public int MinDelaySeconds { get; set; } = 1;
        public int MaxDelaySeconds { get; set; } = 5;
        public int RefundDelaySeconds { get; set; } = 2;
        public string PublicBaseUrl { get; set; } = "http://localhost:5100";
    }

    public class MerchantOptions
    {
        public const string SectionName = "Merchant";

        public string ApiKey { get; set; } = "test_api_key";
        public string WebhookUrl { get; set; } = "http://localhost:5005/webhooks/bank";
        public string WebhookSecret { get; set; } = "test_webhook_secret";
    }
}
