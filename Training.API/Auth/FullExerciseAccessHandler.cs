using MediatR;
using Microsoft.AspNetCore.Authorization;
using Training.Application.Features.Subscription.Queries.HasActiveSubscription;
using Training.Domain;

namespace Training.API.Auth
{
    public class FullExerciseAccessRequirement : IAuthorizationRequirement
    {
    }

    public class FullExerciseAccessHandler : AuthorizationHandler<FullExerciseAccessRequirement>
    {
        private readonly IMediator _mediator;

        public FullExerciseAccessHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            FullExerciseAccessRequirement requirement)
        {
            if (context.User.IsInRole(TrainingRoles.Manager) || context.User.IsInRole(TrainingRoles.Admin))
            {
                context.Succeed(requirement);
                return;
            }

            if (!context.User.IsInRole(TrainingRoles.Premium))
            {
                return;
            }

            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return;
            }

            var active = await _mediator.Send(new HasActiveSubscriptionQuery(userId), CancellationToken.None);
            if (active)
            {
                context.Succeed(requirement);
            }
        }
    }
}
