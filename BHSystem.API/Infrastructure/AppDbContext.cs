using Microsoft.EntityFrameworkCore;
using BHSytem.Models.Entities;
namespace BHSystem.API.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
    }


}
