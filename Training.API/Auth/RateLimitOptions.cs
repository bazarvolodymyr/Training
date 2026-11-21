namespace Training.API.Auth
{
    public class RateLimitOptions
    {
        public const string SectionName = "RateLimit";

        public int AuthPermitLimit { get; set; } = 20;

        public int AuthWindowSeconds { get; set; } = 60;

        public int SubmitPermitLimit { get; set; } = 30;

        public int SubmitWindowSeconds { get; set; } = 60;
    }
}
