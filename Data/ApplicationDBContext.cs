using Microsoft.EntityFrameworkCore;
using CMCS_Web.Models;

namespace CMCS_Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; }   // your existing Claim entity
        public DbSet<User> Users { get; set; }     // add this line
    }
}
