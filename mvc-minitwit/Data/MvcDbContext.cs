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

        public DbSet<Message> Message { get; set; }
        public DbSet<Follower> Follower { get; set; }
        public DbSet<User> User { get; set; }
    }
}