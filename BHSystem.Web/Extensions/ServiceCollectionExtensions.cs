using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSystem.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

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

        /// <summary>
        /// Lưu trạng thái đăng nhập
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            return services;
        }

        /// <summary>
        /// các service Component 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddComponentService(this IServiceCollection services)
        {
            services.AddScoped<BHDialogService>();
            return services;
        }
    }
}
