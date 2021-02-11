using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Models;

namespace mvc_minitwit.Data
{
    public class MvcDbContext : DbContext
    {
        public MvcDbContext (DbContextOptions<MvcDbContext> options)
            : base(options)
        {

        }

protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Follower>(eb =>
                {
                    eb.HasNoKey();
                });
        }


        public DbSet<Message> message { get; set; }
        public DbSet<Follower> follower { get; set; }
        public DbSet<User> user { get; set; }
    }

    
}