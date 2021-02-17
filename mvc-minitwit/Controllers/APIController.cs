using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Api;
using mvc_minitwit.Data;
using mvc_minitwit.Models;
using FromBodyAttribute = System.Web.Http.FromBodyAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;


// http://localhost:5001/API

namespace mvc_minitwit.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
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

        [HttpGet] //https://localhost:5001/api/Getmessage
        //[System.Web.Http.Route("GetMessage")]
        public async Task<ActionResult<IEnumerable<Message>>> Getmessage()
        {
            return await _context.message.OrderByDescending(m => m.pub_date)
                .Take(100)
                .ToListAsync();
        }

        // https://localhost:5001/api/Register?user=bo      with string user as param
        //from uri: https://localhost:5001/api/Register?user_id=42&username=Bo&email=bo@bo.bo&pw_hash=123&pw_hash2=123
        [HttpPost]
        public ActionResult<User> Register([FromUri] User user)
        {
            string error = "";
            //Console.WriteLine("User: " + user.user_id);
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
                return Ok("User registered");
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
