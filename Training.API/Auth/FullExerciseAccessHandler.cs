using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Training.Application.Interfaces;
using Training.Domain;

namespace Training.API.Auth
{
    public class FullExerciseAccessRequirement : IAuthorizationRequirement
    {
    }

    public class FullExerciseAccessHandler : AuthorizationHandler<FullExerciseAccessRequirement>
    {
        private readonly ITrainingDbContext _db;

        public FullExerciseAccessHandler(ITrainingDbContext db)
        {
            _db = db;
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

            var now = DateTimeOffset.UtcNow;
            var active = await _db.Subscriptions.AsNoTracking()
                .AnyAsync(subscription => subscription.UserId == userId && subscription.ExpiresAt > now);
            if (active)
            {
                context.Succeed(requirement);
            }
        }
    }
}
