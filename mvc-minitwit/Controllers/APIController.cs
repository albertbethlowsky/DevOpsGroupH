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

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


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
            if (user.Count() == 1)
            {
                return user.Single().user_id;
            }
            else return -1;     //no user found
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

            ApiDataLatest returnlatest = new ApiDataLatest {latest = LATEST};
            var jsonreturn = JsonSerializer.Serialize(returnlatest);
            return Ok(jsonreturn);
        }

        //This is working now the - /msgs?no=42
        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAllMessages(int no = 100)
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

        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetMessageByUserAndItsFollowers(string username, int no = 100)
        {
            UpdateLatest();
            var userId = GetUserId(username);
            if (userId == -1) return BadRequest("invalid username");
            var checkfollow = (from f in _context.follower
                               join u in _context.user on f.whom_id equals u.user_id
                               select
                               new Follower { who_id = f.who_id, whom_id = f.whom_id, whom_name = u.username }).Where(i => i.who_id == userId).ToList();
            var followlist = new List<Int32>() { userId };
            foreach (var item in checkfollow)
                followlist.Add(item.whom_id);

            var joinedtable = await (from m in _context.message
                                     where followlist.Contains(m.author_id)
                                     select
                                   new 
                                   {
                                       content = m.text,
                                       pub_date = m.pub_date,
                                       user = m.author.username
                                   }).Distinct()
                                     .OrderByDescending(x => x.pub_date)
                                     .Take(no)
                                     .ToListAsync();
                                           

            return joinedtable;

        }


        //This is working now - /msgs/<username>
        [HttpPost("{username}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> CreateMessageByUser(string username,[FromBody] CreateMessage model)
        {
            Message message = new Message();
            message.author_id = _context.user.Single(x => x.username == username).user_id;
            message.text = model.content;
            message.pub_date = (Int32)(DateTimeOffset.Now.ToUnixTimeSeconds());
            message.flagged = 0;

            _context.message.Add(message);
            await _context.SaveChangesAsync();
            return Ok("Message posted");
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
            else if (GetUserId(user.username) != -1)
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
            if (!string.IsNullOrEmpty(error))
            {
                var ls = _context.user.ToList().Select(u => u.username);
                var str = string.Join(",", ls.ToList());
                return BadRequest(error);

            }
            else
            {
                var noUsers = _context.user;
                var ls = noUsers.ToList().Select(u => u.username);
                var str = string.Join(",", ls.ToList());
                return Ok("User registered " + user.username);

            }
        }

        [HttpPost]
        [Route("~/api/SignIn")]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            var f_password = new GravatarImage().hashBuilder(password);

            User user = await _context.user.FirstOrDefaultAsync(u => u.email.Equals(email) && u.pw_hash.Equals(f_password));
            if(user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("UserEmail", user.email),
                    new Claim("Username", user.username),
                    new Claim("UserID", user.user_id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Ok("You were logged in");
            
            } else {
                return BadRequest("Wrong email or password");
            }

        }
        
        [HttpPost]
        [Route("~/api/SignOut")]
        public async Task<IActionResult> Sign_Out()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [Route("~/fllws/{username}")]
        [AcceptVerbs("POST", "GET")]
        public IActionResult Follow(string username, int no_followers = 100) {
            var verb = _accessor.HttpContext.Request.Method.ToString();
            var json = _accessor.HttpContext.Request.ReadFromJsonAsync<ApiDataFollow>();
            int userid = GetUserId(username);
            
            UpdateLatest();
            if(userid == -1) return BadRequest("error");

            if (verb == "POST" && json.Result.follow != null){
                string userToFollow = json.Result.follow;
                int userToFollowId = GetUserId(userToFollow);  //find the id of user to follow
                if(userToFollowId == -1) return NotFound();

                var followersOfUserId = _context.follower.Where(f => f.who_id == userid).ToList();
                
                if (followersOfUserId.Where(f => f.whom_id == userToFollowId).Any())
                {
                    return BadRequest(username + " already follows " + userToFollow);
                }

                Follower follower = new Follower();
                follower.who_id = userid;
                follower.whom_id = userToFollowId;
                _context.Add(follower);
                _context.SaveChanges();

                return Ok(username + " now follows " + userToFollow);
                
            } else if(verb == "POST" && json.Result.unfollow != null) {
                string userToUnfollow = json.Result.unfollow;
                int userToUnfollowId = GetUserId(userToUnfollow);
                if(userToUnfollowId == -1) return NotFound();

                var followersOfUserId = _context.follower.AsNoTracking().Where(f => f.who_id == userid).ToList();
                
                if (!followersOfUserId.Where(f => f.whom_id == userToUnfollowId).Any())
                {
                    return BadRequest(username + " isn't following " + userToUnfollow + " to begin with");
                }

                Follower follower = new Follower();
                follower.who_id = userid;
                follower.whom_id = userToUnfollowId;
                _context.Remove(follower);
                _context.SaveChanges();

                return Ok(username + " now doesn't follow " + userToUnfollow);

        
            } else if(verb == "GET"){ //needs refactoring to use ORM instead of query
                var query = (from f in _context.follower

                                    join u in _context.user on f.whom_id equals u.user_id
                                    where f.who_id == userid
                                    select 
                                    new {u.username})
                                    .Take(no_followers).ToList();
                List<string> Follows = new List<string>();
                foreach (var item in query)
                {
                    Follows.Add(item.username);
                }                
                ApiDataFollows returnfollows = new ApiDataFollows {follows = Follows};
                var jsonreturn = JsonSerializer.Serialize(returnfollows);

                return Ok(jsonreturn);
            }
            return Ok("other");
        }

    }
}

