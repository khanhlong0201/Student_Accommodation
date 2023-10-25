using BHSystem.API.Services;

namespace BHSystem.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientScopeService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }

    }
}
