namespace Training.API.Auth
{
    public class AuthOptions
    {
        public const string SectionName = "Auth";

        public string UiOrigin { get; set; } = "http://localhost:4200";

        public string AdminEmail { get; set; } = "admin@training.local";

        public GoogleAuthOptions Google { get; set; } = new();
    }

    public class GoogleAuthOptions
    {
        public string Mode { get; set; } = "Stub";

        public string Email { get; set; } = "google.user@example.com";

        public string Name { get; set; } = "Google User";

        public string ProviderKey { get; set; } = "stub-google-user";

        public bool IsStub => string.Equals(Mode, "Stub", StringComparison.OrdinalIgnoreCase);
    }
}
