using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain.Models;

namespace Training.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> subscription)
        {
            subscription.ToTable("Subscriptions");
            subscription.HasKey(x => x.Id);
            subscription.HasIndex(x => new { x.UserId, x.ExpiresAt });
            subscription.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            subscription.HasOne<TrainingUser>()
                .WithMany()
                .HasForeignKey(x => x.IssuedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
