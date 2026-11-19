namespace Training.Domain
{
    public static class TrainingRoles
    {
        public const string Standard = "Standard";
        public const string Premium = "Premium";
        public const string Manager = "Manager";
        public const string Admin = "Admin";

        public static readonly IReadOnlyList<string> All =
        [
            Standard,
            Premium,
            Manager,
            Admin
        ];
    }
}
