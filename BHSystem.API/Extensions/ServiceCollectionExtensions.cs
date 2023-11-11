using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSystem.API.Services;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Extensions
{
    public static class ServiceCollectionExtensions // DI Extensions  (đăng ký serice,repo)
    {
        public static IServiceCollection AddScopeRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsersRepository, UsersRepository>(); //AddScoped mội đối tượng mới dc tạo ra khi có yêu cầu HTTP và bị hủy khi kết thúc yêu cầu
            services.AddScoped<IMenusRepository, MenusRepository>();
            services.AddScoped<IWardsRepository, WardsRepository>();
            services.AddScoped<IUserRolesRepository, UserRolesRepository>();
            services.AddScoped<IRoomsRepository, RoomsRepository>();
            services.AddScoped<IRoomPricesRepository, RoomPricesRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<IRoleMenusRepository, RoleMenusRepository>();
            services.AddScoped<IImagesRepository, ImagesRepository>();
            services.AddScoped<IImagesDetailsRepository, ImagesDetailsRepository>();
            services.AddScoped<IDistinctsRepository, DistinctsRepository>();
            services.AddScoped<ICommentsRepository, CommentsRepository>();
            services.AddScoped<ICitysRepository, CitysRepository>();
            services.AddScoped<IBookingsRepository, BookingsRepository>();
            services.AddScoped<IBoardingHousesRepository, BoardingHousesRepository>();
            services.AddScoped<IBHousesRepository, BHousesRepository>();
            return services;
        }
        public static IServiceCollection AddClientScopeService(this IServiceCollection services)
        {
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IMenusService, MenusService>();
            services.AddScoped<IWardsService, WardsService>();
            services.AddScoped<IUserRolesService, UserRolesService>();
            services.AddScoped<IRoomsService, RoomsService>();
            services.AddScoped<IRoomPricesService, RoomPricesService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IRoleMenusService, RoleMenusService>();
            services.AddScoped<IMenusService, MenusService>();
            services.AddScoped<IImagesService, ImagesService>();
            services.AddScoped<IImagesDetailsService, ImagesDetailsService>();
            services.AddScoped<IDistinctsService, DistinctsService>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<ICitysService, CitysService>();
            services.AddScoped<IBookingsService, BookingsService>();
            services.AddScoped<IBoardingHousesService, BoardingHousesService>();
            services.AddScoped<IBHousesService, BHousesService>();
            return services;
        }

        //public static ModelBuilder SeedData(this ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Citys>().HasData(
        //       new Citys()
        //       {
        //           Id = 1
        //       ,
        //           Name = "HCM"
        //       ,
        //           IsDeleted = true
        //       });
        //    modelBuilder.Entity<Distincts>().HasData(
        //       new Distincts()
        //       {
        //           Id = 1
        //       ,
        //           Name = "Ho Chi Minh"
        //       ,
        //           City_Id = 1
        //           ,
        //           IsDeleted = true
        //       });

        //    modelBuilder.Entity<Wards>().HasData(
        //       new Wards()
        //       {
        //           Id = 1
        //       ,
        //           Name = "Go Vap"
        //       ,
        //           Distincts_Id = 1
        //           ,
        //           IsDeleted = true
        //       });

        //    modelBuilder.Entity<Users>().HasData(
        //        new Users()
        //        {
        //            UserId = 1
        //        ,
        //            FullName = "HaiNguyen"
        //        ,
        //            UserName = "hainguyen456"
        //        ,
        //            Password = "KZY9mwl2Mv4NM4jrKXv4ug=="
        //        ,
        //            Ward_Id = 1

        //        ,
        //            IsDeleted = true
        //        ,   Type = "Admin"
        //        });
        //    return modelBuilder;
        //}
    }
}
