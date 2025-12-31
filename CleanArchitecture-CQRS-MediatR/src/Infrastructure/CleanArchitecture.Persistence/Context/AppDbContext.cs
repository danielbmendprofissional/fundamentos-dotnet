using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CleanArchitecture.Persistence.Context
{
    /*
        EF CORE context file
    */
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
