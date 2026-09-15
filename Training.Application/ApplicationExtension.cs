using Microsoft.Extensions.DependencyInjection;
using Training.Application.Services;
using Training.Domain.Interfaces.Services;

namespace Training.Application
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IExerciseService, ExerciseService>();

            return services;
        }
    }
}
