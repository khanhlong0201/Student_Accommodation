using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSystem.API.Services;

namespace BHSystem.API.Extensions
{
    public static class ServiceCollectionExtensions // DI Extensions  (đăng ký serice)
    {
        public static IServiceCollection AddScopeRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            return services;
        }
        public static IServiceCollection AddClientScopeService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMenuService, MenuService>();
            return services;
        }

    }
}
