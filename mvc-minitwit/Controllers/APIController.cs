using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Api;
using mvc_minitwit.Data;
using mvc_minitwit.HelperClasses;
using mvc_minitwit.Models;
using System.Text.Json;
using System.Text.Json.Serialization;


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
            return Ok(LATEST);
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
                _context.user.Add(user);
                await _context.SaveChangesAsync();
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

        [Route("~/fllws/{username}")]
        [AcceptVerbs("POST", "GET")]
        //@app.route("/fllws/<username>", methods=["GET", "POST"])
        public IActionResult follow(string username, int no_followers = 100) {

            var verb = _accessor.HttpContext.Request.Method.ToString();
            Console.WriteLine(_accessor.HttpContext.Request.ReadFromJsonAsync<ActionResult<User>>());
            json = JsonSerializer.Deserialize(_accessor.HttpContext.Request.ReadFromJsonAsync<>());
            
            UpdateLatest();

            if(GetUserId(username) == -1) return BadRequest("error");
            
            if (verb == "POST" && json == "follow"){
                
                // _context.follower.Where(f => )

                //FROM PYTHON:
                // follows_username = request.json["follow"]

                // follows_user_id = get_user_id(follows_username)
                // if not follows_user_id:
                //     abort(404)

                // query = """INSERT INTO follower (who_id, whom_id) VALUES (?, ?)"""
                // g.db.execute(query, [user_id, follows_user_id])
                // g.db.commit()

                // return "", 204
                
            } else if(verb == "POST" && json=="unfollow") {
                Console.WriteLine("Returns Something");
                //FROM PYTHON:
                // unfollows_username = request.json["unfollow"]
                // unfollows_user_id = get_user_id(unfollows_username)
                // if not unfollows_user_id:
                //     abort(404)

                // query = "DELETE FROM follower WHERE who_id=? and WHOM_ID=?"
                // g.db.execute(query, [user_id, unfollows_user_id])
                // g.db.commit()
                //return "", 204

        
            } else if(verb == "GET"){
                //FROM PYTHON:
                // no_followers = request.args.get("no", type=int, default=100)
                // query = """SELECT user.username FROM user
                //         INNER JOIN follower ON follower.whom_id=user.user_id
                //         WHERE follower.who_id=?
                //         LIMIT ?"""
                // followers = query_db(query, [user_id, no_followers])
                // follower_names = [f["username"] for f in followers]
                // followers_response = {"follows": follower_names}

                // return jsonify(followers_response)
            }
            return Ok();
        
        }

    }
}

