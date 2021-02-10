using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Models;

namespace mvc_minitwit.Data
{
    public class MessageContext : DbContext
    {
        public MessageContext (DbContextOptions<MessageContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Message { get; set; }
    }
}