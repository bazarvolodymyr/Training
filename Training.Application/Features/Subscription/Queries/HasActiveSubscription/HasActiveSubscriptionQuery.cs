using MediatR;

namespace Training.Application.Features.Subscription.Queries.HasActiveSubscription
{
    public record HasActiveSubscriptionQuery(string UserId) : IRequest<bool>;
}
