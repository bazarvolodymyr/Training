using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Training.Domain;
using Training.Domain.Models;

namespace Training.API.Auth
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roles;
        private readonly UserManager<TrainingUser> _users;
        private readonly AuthOptions _auth;

        public IdentitySeeder(
            RoleManager<IdentityRole> roles,
            UserManager<TrainingUser> users,
            IOptions<AuthOptions> auth)
        {
            _roles = roles;
            _users = users;
            _auth = auth.Value;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            foreach (var role in TrainingRoles.All)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!await _roles.RoleExistsAsync(role))
                {
                    var created = await _roles.CreateAsync(new IdentityRole(role));
                    if (!created.Succeeded)
                    {
                        throw new InvalidOperationException($"Не вдалося створити роль {role}.");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(_auth.AdminEmail))
            {
                return;
            }

            var admin = await _users.FindByEmailAsync(_auth.AdminEmail);
            if (admin is null)
            {
                admin = new TrainingUser
                {
                    UserName = _auth.AdminEmail,
                    Email = _auth.AdminEmail,
                    EmailConfirmed = true
                };
                var created = await _users.CreateAsync(admin);
                if (!created.Succeeded)
                {
                    throw new InvalidOperationException("Не вдалося створити користувача Admin.");
                }
            }

            if (!await _users.IsInRoleAsync(admin, TrainingRoles.Admin))
            {
                var assigned = await _users.AddToRoleAsync(admin, TrainingRoles.Admin);
                if (!assigned.Succeeded)
                {
                    throw new InvalidOperationException("Не вдалося призначити роль Admin.");
                }
            }
        }
    }
}
