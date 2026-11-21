using Microsoft.Extensions.DependencyInjection;
using Training.Application.Features.Lessons;

namespace Training.Application
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtension).Assembly));
            services.AddSingleton<ArticleHtmlSanitizer>();

            return services;
        }
    }
}
