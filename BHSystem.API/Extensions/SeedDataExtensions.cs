using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Extensions
{
    public static class SeedDataExtensions
    {
        public static ModelBuilder SeedDataMenus(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Menus>().HasData(
                new Menus() {MenuId = "000-001", Name = "Biểu đồ thống kê", Icon = "fa fa-chart-bar", Link= "/admin/report", Parent= ""},
                new Menus() {MenuId = "000-002", Name = "Quản lý phòng trọ", Icon = "fa-solid fa-people-roof", Link= "/admin/boarding-house", Parent = "" },
                new Menus() {MenuId = "000-003", Name = "Phê duyệt phòng", Icon = "fa-solid fa-user-check", Link= "/admin/approve-room", Parent = "" },
                new Menus() {MenuId = "000-004", Name = "Xác nhận đặt phòng", Icon = "fa-solid fa-user-pen", Link= "/admin/approve-booking", Parent = "" },
                new Menus() {MenuId = "000-005", Name = "Quản lý người dùng", Icon = "fa-solid fa-users", Link= "/admin/user", Parent = "" },
                new Menus() {MenuId = "000-006", Name = "Quản lý nhóm quyền", Icon = "fa-solid fa-folder-tree", Link= "/admin/role", Parent = "" },
                new Menus() {MenuId = "000-007", Name = "Theo dõi thông báo", Icon = "fa-solid fa-bell", Link= "/admin/message", Parent = "" }
                );
            return modelBuilder;
        }

        public static ModelBuilder SeedDataRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Roles>().HasData(
                new Roles() { Id = 1, Name = "Admintrator"}, // default
                new Roles() { Id = 2, Name = "Bussiness Partner" },
                new Roles() { Id = 3, Name = "Client" }
                );
            return modelBuilder;
        }

        public static ModelBuilder SeedDataUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasData(
                new Users(){ UserId = 1,FullName = "Khách Vãn Lai",UserName = "",Password = "",Ward_Id = 1,IsDeleted = false,Type = "Client"}, // default
                new Users(){ UserId = 2,FullName = "Testing",UserName = "testing",Password = "KZY9mwl2Mv4NM4jrKXv4ug==",Ward_Id = 1,IsDeleted = false, Type = "Admin"}
                );
            return modelBuilder;
        }

        public static ModelBuilder SeedDataRoleMenus(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleMenus>().HasData(
                new RoleMenus() { Role_Id = 1, Menu_Id = "000-001" },
                new RoleMenus() { Role_Id = 1, Menu_Id = "000-002" },
                new RoleMenus() { Role_Id = 1, Menu_Id = "000-003" },
                new RoleMenus() { Role_Id = 1, Menu_Id = "000-004" },
                new RoleMenus() { Role_Id = 1, Menu_Id = "000-005" },
                new RoleMenus() { Role_Id = 1, Menu_Id = "000-006" },
                new RoleMenus() { Role_Id = 1, Menu_Id = "000-007" }
                );
            return modelBuilder;
        }

        public static ModelBuilder SeedDataRoleUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRoles>().HasData(
                new UserRoles() { Role_Id = 1, UserId = 2 }
                );
            return modelBuilder;
        }
    }
}
