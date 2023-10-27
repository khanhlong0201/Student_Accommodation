using BHSytem.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
    }


}
