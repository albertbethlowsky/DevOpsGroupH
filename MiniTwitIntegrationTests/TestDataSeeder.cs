using mvc_minitwit.Data;
using mvc_minitwit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTwitIntegrationTests
{
    class TestDataSeeder
    {
        public const string FirstItemId = "312658D1-8146-42E3-B57B-360427182811";
        public const string SecondItemId = "64C7E3F5-74F9-4540-9B12-BC7AFBCC7CE6";

        public static readonly User user1 = new User() { username = "name", email= "name@name", pw_hash="hash" };
        public static readonly User user2 = new User() { username = "name2", email = "name2@name2", pw_hash = "hash" };

        private readonly MvcDbContext _context;

        public TestDataSeeder(MvcDbContext context)
        {
            _context = context;

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public void SeedToDoItems()
        {
            _context.user.Add(user1);
            _context.user.Add(user2);
            _context.SaveChanges();
        }
    }
}
