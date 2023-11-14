using Microsoft.EntityFrameworkCore;
using BHSytem.Models.Entities;
using BHSystem.API.Extensions;

namespace BHSystem.API.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<RoleMenus> RoleMenus { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<RoomPrices> RoomPrices { get; set; }
        public DbSet<BoardingHouses> BoardingHouses { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Wards> Wards { get; set; }
        public DbSet<Distincts> Distincts { get; set; }
        public DbSet<Citys> Citys { get; set; }
        public DbSet<ImagesDetails> ImagesDetails { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<UserMessages> UserMessages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        //map khóa ngoại và khóa chính
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Citys>() .Property(e => e.Id).ValueGeneratedNever(); // Tắt tính năng tự tăng
            modelBuilder.Entity<Distincts>().Property(e => e.Id).ValueGeneratedNever(); // Tắt tính năng tự tăng
            modelBuilder.Entity<Wards>().Property(e => e.Id).ValueGeneratedNever(); // Tắt tính năng tự tăng

            modelBuilder.Entity<Distincts>().HasOne(i => i.Citys).WithMany(c => c.Distincts).HasForeignKey(i => i.City_Id);
            modelBuilder.Entity<Wards>().HasOne(i => i.Distincts).WithMany(c => c.Wards).HasForeignKey(i => i.Distincts_Id);
            modelBuilder.Entity<Users>().HasOne(i => i.Wards).WithMany(c => c.Users).HasForeignKey(i => i.Ward_Id);

            modelBuilder.Entity<RoleMenus>().HasKey(i => new { i.Role_Id, i.Menu_Id }); //định nghĩa 2 khóa chính cùng 1 bảng
            modelBuilder.Entity<RoleMenus>().HasOne(i => i.Roles).WithMany(c => c.RoleMenus).HasForeignKey(i => i.Role_Id); // định nghĩa khóa ngoại
            modelBuilder.Entity<RoleMenus>().HasOne(i => i.Menus).WithMany(c => c.RoleMenus).HasForeignKey(i => i.Menu_Id).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRoles>().HasKey(i => new { i.UserId, i.Role_Id });
            modelBuilder.Entity<UserRoles>().HasOne(i => i.Roles).WithMany(c => c.UserRoles).HasForeignKey(i => i.Role_Id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserRoles>().HasOne(i => i.Users).WithMany(c => c.UserRoles).HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bookings>().HasOne(i => i.Users).WithMany(c => c.Bookings).HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Bookings>().HasOne(i => i.Rooms).WithMany(c => c.Bookings).HasForeignKey(i => i.Room_Id).OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<RoomPrices>().HasKey(i => new { i.Id, i.Room_Id });
            modelBuilder.Entity<RoomPrices>().HasOne(i => i.Rooms).WithMany(c => c.RoomPrices).HasForeignKey(i => i.Room_Id);

            modelBuilder.Entity<Rooms>().HasOne(i => i.BoardingHouses).WithMany(c => c.Rooms).HasForeignKey(i => i.Boarding_House_Id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rooms>().HasOne(i => i.Images).WithMany(c => c.Rooms).HasForeignKey(i => i.Image_Id).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BoardingHouses>().HasOne(i => i.Users).WithMany(c => c.BoardingHouses).HasForeignKey(i => i.User_Id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BoardingHouses>().HasOne(i => i.Wards).WithMany(c => c.BoardingHouses).HasForeignKey(i => i.Ward_Id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BoardingHouses>().HasOne(i => i.Images).WithMany(c => c.BoardingHouses).HasForeignKey(i => i.Image_Id).OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ImagesDetails>().HasKey(i => new { i.Id, i.Image_Id });
            modelBuilder.Entity<ImagesDetails>().HasOne(i => i.Image).WithMany(c => c.ImagesDetails).HasForeignKey(i => i.Image_Id);
            
            modelBuilder.Entity<Comments>().HasOne(i => i.Image).WithMany(c => c.Comments).HasForeignKey(i => i.Image_Id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Comments>().HasOne(i => i.BoardingHouses).WithMany(c => c.Comments).HasForeignKey(i => i.BoardingHouse_Id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Comments>().HasOne(i => i.Users).WithMany(c => c.Comments).HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMessages>().HasKey(m => new { m.Message_Id, m.UserId});
            //// seed data
            //modelBuilder.SeedDataMenus();
            //modelBuilder.SeedDataRoles();
            //modelBuilder.SeedDataUsers();
            //modelBuilder.SeedDataRoleMenus();
            //modelBuilder.SeedDataRoleUsers();
            base.OnModelCreating(modelBuilder);
        }
    }
}

//lệnh chạy migration
//dotnet ef migrations add DropColumnWithIdentity
//dotnet ef database update

// add-migration initial
// update-database