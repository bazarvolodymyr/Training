using Microsoft.Extensions.DependencyInjection;

namespace Training.Application
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtension).Assembly));

            return services;
        }
    }
}
