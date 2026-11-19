namespace Training.Domain.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public TrainingUser? User { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public string? IssuedByUserId { get; set; }
    }
}
