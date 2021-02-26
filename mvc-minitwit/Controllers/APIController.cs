﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using mvc_minitwit.Data;
using mvc_minitwit.HelperClasses;
using mvc_minitwit.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;


namespace mvc_minitwit.Controllers
{
    [Route("msgs")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly MvcDbContext _context;
        static int LATEST = 0;

        private HttpContextAccessor _accessor = new HttpContextAccessor();

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

        private void UpdateLatest() {
            var query1 = _accessor.HttpContext.Request.Query;
            foreach (var item in query1)
            {
                if(item.Key == "latest") LATEST = Int32.Parse(item.Value);
            }
        }

        [HttpGet("~/latest")]
        public ActionResult GetLatest() {

            ApiData returnlatest = new ApiData {latest = LATEST};
            var jsonreturn = JsonSerializer.Serialize(returnlatest);
            return Ok(jsonreturn);
        }

        //This is working now the - /msgs?no=42
        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> Getmessage(int no = 100)
        {
            UpdateLatest();

            return await _context.message.OrderByDescending(m => m.pub_date)
                .Include(x => x.author)
                .Where(x => x.flagged == 0)
                .OrderByDescending(x => x.pub_date)
                .Select(x => new { content = x.text, pub_date = x.pub_date, user = x.author.username })
                .Take(no)
                .ToListAsync();
        }

        //This is working now - /msgs/<username>
        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetmessageByUser(string username, int no = 100)
        {
            return await _context.message.OrderByDescending(m => m.pub_date)
                .Include(x => x.author)
                .Where(x => x.flagged == 0)
                .Where(x => x.author.username == username)
                .OrderByDescending(x => x.pub_date)
                .Select(x => new { content = x.text, pub_date = x.pub_date, user = x.author.username })
                .Take(no)
                .ToListAsync();
        }

        //This is working now - /msgs/<username>
        [HttpPost("{username}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> CreatemessageByUser(string username, CreateMessage model)
        {
            Message message = new Message();
            message.author_id = _context.user.Single(x => x.username == username).user_id;
            message.text = model.Content;
            message.pub_date = (Int32)(DateTimeOffset.Now.ToUnixTimeSeconds());
            message.flagged = 0;

            _context.message.Add(message);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("~/register")] //This syntax goes back to root and the /whaterver
        public async Task<ActionResult<User>> Register([FromBody] User user)
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
                //The user given in the json body from the request, 
                //isn't added directly to  the context (that would insecure). But its attributes are used such that
                //userId is generated automatically, and the pw is hashed into pw_hash
                _context.user.Add(new User { username = user.username, email = user.email, pw_hash = new GravatarImage().hashBuilder(user.pw_hash)});
                await _context.SaveChangesAsync();
            }

            if (string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }
            else
            {
                var noUsers = _context.user.Count();
                return Ok("User registered no: " +  noUsers);
            }
        }

        [Route("~/fllws/{username}")]
        [AcceptVerbs("POST", "GET")]
        public IActionResult follow(string username, int no_followers = 100) {
            var verb = _accessor.HttpContext.Request.Method.ToString();
            var json = _accessor.HttpContext.Request.ReadFromJsonAsync<ApiData>();
            int userid = GetUserId(username);
            
            UpdateLatest();

            if(userid == -1) return BadRequest("error");
            
            if (verb == "POST" && json.Result.follow != null){
                string follows_username = json.Result.follow;
                int follows_user_id = GetUserId(follows_username);
                if(follows_user_id == -1) return NotFound();

                Follower follower = new Follower();
                follower.who_id = userid;
                follower.whom_id = follows_user_id;
                _context.Add(follower);
                _context.SaveChanges();

            return Ok();
                
            } else if(verb == "POST" && json.Result.unfollow != null) {
                string follows_username = json.Result.unfollow;
                int follows_user_id = GetUserId(follows_username);
                if(follows_user_id == -1) return NotFound();
        
                Follower follower = new Follower();
                follower.who_id = userid;
                follower.whom_id = follows_user_id;
                _context.Remove(follower);
                _context.SaveChanges();

                return Ok();

        
            } else if(verb == "GET"){ //needs refactoring to use ORM instead of query
                var query = (from f in _context.follower
                                    join u in _context.user on f.whom_id equals u.user_id
                                    where f.who_id == userid
                                    select 
                                    new {follows = u.username})
                                    .Take(no_followers).ToList();
                var jsonreturn = JsonSerializer.Serialize(query);

                return Ok(jsonreturn);
            }
            return Ok();
        }

    }
}

