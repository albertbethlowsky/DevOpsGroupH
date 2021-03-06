using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mvc_minitwit.Data;
using System;
using System.Linq;

namespace mvc_minitwit.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<MvcDbContext>>()))
            {
                // Look for any messages.
                if (context.message.Any())
                {
                    Console.WriteLine("the db is not empty!") ;   // DB has been seeded
                    
                    return;
                } else {
                    Console.WriteLine("db it not found/or empty");
                    return;
                }

                //add something to db?
            }
        }
    }

    
}