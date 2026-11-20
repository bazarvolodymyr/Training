using MediatR;
using Microsoft.AspNetCore.Identity;
using Training.Domain;
using Training.Domain.Models;

namespace Training.Application.Features.Account.Commands.ErasePersonalData
{
    public class ErasePersonalDataCommandHandler : IRequestHandler<ErasePersonalDataCommand, ErasePersonalDataStatus>
    {
        private readonly UserManager<TrainingUser> _users;

        public ErasePersonalDataCommandHandler(UserManager<TrainingUser> users)
        {
            _users = users;
        }

        public async Task<ErasePersonalDataStatus> Handle(ErasePersonalDataCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.FindByIdAsync(request.UserId);
            if (user is null)
            {
                return ErasePersonalDataStatus.NotFound;
            }

            if (await _users.IsInRoleAsync(user, TrainingRoles.Admin))
            {
                var admins = await _users.GetUsersInRoleAsync(TrainingRoles.Admin);
                if (admins.Count <= 1)
                {
                    return ErasePersonalDataStatus.LastAdmin;
                }
            }

            // QuizAttempt і UserLessonProgress ще не існують. Нові персональні таблиці з UserId видаляти тут, до акаунта.
            // Підписки цього користувача зникають каскадом. Уроки й чужі підписки (IssuedBy) лишаються.
            await _users.UpdateSecurityStampAsync(user);
            var deleted = await _users.DeleteAsync(user);
            if (!deleted.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", deleted.Errors.Select(error => error.Description)));
            }

            return ErasePersonalDataStatus.Erased;
        }
    }
}
