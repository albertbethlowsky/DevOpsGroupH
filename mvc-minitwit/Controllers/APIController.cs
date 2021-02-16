using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Api;
using mvc_minitwit.Data;
using mvc_minitwit.Models;


// http://localhost:5001/API

namespace mvc_minitwit.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly MvcDbContext _context;

        public APIController(MvcDbContext context)
        {
            _context = context;
        }

        private int GetUserId(string username)
        {
            var user = _context.user.Where(u => u.username == username);
            if (user.SingleOrDefault() != null)
            {
                return user.Single().user_id;
            }
            else return -1;
        }

        [HttpGet("{msgs}")] //https://localhost:5001/msgs
        public async Task<ActionResult<IEnumerable<Message>>> Getmessage()
        {
            return await _context.message.OrderByDescending(m => m.pub_date)
                .Take(100)
                .ToListAsync();
        }

        [HttpPost("{username}")]
        public async Task<ActionResult<User>> Register(User user)
        {
            string error = "";
            if (string.IsNullOrEmpty(user.username))
            {
                error = "You have to enter a username";
            }
            else if (string.IsNullOrEmpty(user.email) || !user.email.Contains("@"))
            {
                error = "You have to enter a valid email address";
            }
            else if (string.IsNullOrEmpty(user.pw_hash))
            {
                error = "You have to enter a password";
            }
            else if (GetUserId(user.username) == -1)
            {
                error = "The username is already taken";
            }
            else
            {
                _context.user.Add(user);
                _context.SaveChanges();
            }

            if (string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }
            else
            {
                return Ok();
            }

        }



        //MESSAGES API:
        // GET: /Message????
        // [HttpGet]
        // [Route("/")]
        // public async Task<ActionResult<IEnumerable<Message>>> Getmessage()
        // {
        //     return MessagesController.Getmessage(); //returns task
        // }

    }
}
