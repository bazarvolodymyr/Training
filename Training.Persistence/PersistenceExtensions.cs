using Microsoft.Extensions.DependencyInjection;
using Training.Domain.Interfaces.Repositories;
using Training.Persistence.Repositories;

namespace Training.Persistence
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IExerciseRepository, ExerciseRepository>();

            return services;
        }
    }
}
