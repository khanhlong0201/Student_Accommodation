using BHSystem.Web.Core;
using BHSystem.Web.Services;

namespace BHSystem.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// HttpClient Services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientScopeService(this IServiceCollection services)
        {
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<ICliUserService, CliUserService>();
            return services;
        }
    }
}
