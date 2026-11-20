using Microsoft.AspNetCore.Identity;

namespace Training.Domain.Models
{
    public class TrainingUser : IdentityUser
    {
        public DateTimeOffset? AcceptedTermsAt { get; set; }
    }
}
