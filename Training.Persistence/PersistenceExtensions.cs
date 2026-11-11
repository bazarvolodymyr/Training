using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.Application.Interfaces;

namespace Training.Persistence
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Training")
                ?? throw new InvalidOperationException(
                    "Connection string 'Training' is not configured. Set ConnectionStrings:Training via user secrets.");

            services.AddDbContext<TrainingDbContext>(options =>
                    options.UseNpgsql(connectionString));
            services.AddScoped<ITrainingDbContext>(sp => sp.GetRequiredService<TrainingDbContext>());

            return services;
        }
    }
}
