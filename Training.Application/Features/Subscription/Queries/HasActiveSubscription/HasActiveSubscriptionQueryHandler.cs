using MediatR;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;

namespace Training.Application.Features.Subscription.Queries.HasActiveSubscription
{
    public class HasActiveSubscriptionQueryHandler : IRequestHandler<HasActiveSubscriptionQuery, bool>
    {
        private readonly ITrainingDbContext _db;

        public HasActiveSubscriptionQueryHandler(ITrainingDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Handle(HasActiveSubscriptionQuery request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            return await _db.Subscriptions.AsNoTracking()
                .AnyAsync(
                    subscription => subscription.UserId == request.UserId && subscription.ExpiresAt > now,
                    cancellationToken);
        }
    }
}
