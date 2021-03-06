using mvc_minitwit.Data;
using mvc_minitwit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControllerTests
{
    public class SeedData
    {
        public static User user { get; set; }
        public static Message message { get; set; }

        public void PopulateTestData(MvcDbContext dbContext)
        {
            user = new User { username = "SeedUser", pw_hash = "somehash", pw_hash2 = "somehash", email = "seed@seed" };
            dbContext.user.Add(user);
            message = new Message { author_id = 0, author = user, text = "seed message" };
            dbContext.message.Add(message);

            dbContext.SaveChanges();
        }
    }
}
