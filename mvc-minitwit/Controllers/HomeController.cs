using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Models;
using mvc_minitwit.Data;
using mvc_minitwit.HelperClasses;


namespace mvc_minitwit.Controllers
{
    public class HomeController : Controller
    
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly MvcDbContext _context;

        public HomeController(ILogger<HomeController> logger, MvcDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Timeline(string? id)
        {    
            LoginHelper lh = new LoginHelper();
            if(id == "My Timeline") {
                

                ViewData["Title"] = "My Timeline";
                var joinedtable = (from m in _context.message
                                join u in _context.user on m.author_id equals u.user_id
                                join f in _context.follower on u.user_id equals f.who_id
                                select
                                new TimelineData {message_id = m.message_id, email = u.email, username = u.username, text = m.text, pub_date = m.pub_date, who_id = f.who_id}).Where(u => u.who_id == Int32.Parse(lh.getUserID())).OrderByDescending(t => t.message_id).Take(50).ToList();

                return View(joinedtable);

            } else if(id == "Public Timeline" || id == null) {

                ViewData["Title"] = "Public Timeline";
                var joinedtable = (from m in _context.message
                                join u in _context.user on m.author_id equals u.user_id
                                select 
                                new TimelineData {message_id = m.message_id, email = u.email, username = u.username, text = m.text, pub_date = m.pub_date}).OrderByDescending(t => t.message_id).Take(50).ToList();

                return View(joinedtable);

            } else {

                ViewData["Title"] = id + "'s Timeline";

                var joinedtable = (from m in _context.message
                                join u in _context.user on m.author_id equals u.user_id
                                select
                                new TimelineData {message_id = m.message_id, author_id = m.author_id, email = u.email, username = u.username, text = m.text, pub_date = m.pub_date, isFollowed = false}).Where(u => u.username == id).OrderByDescending(t => t.message_id).Take(50).ToList();
                if(lh.checkLogin())
                {
                    var checkfollow = (from f in _context.follower
                                    join u in _context.user on f.whom_id equals u.user_id
                                    where u.username == id && f.who_id == Int32.Parse(lh.getUserID())
                                    select 
                                    new Follower {who_id = f.who_id, whom_id = f.whom_id}).ToList();

                    if(checkfollow.Any()) {
                        joinedtable[0].isFollowed = true;
                    }
                }

                return View(joinedtable);
            }
        }
        [HttpPost]
        public IActionResult Message(string text)
        {
            Message message = new Message();
            LoginHelper lh = new LoginHelper();
            Console.WriteLine(text);
            message.author_id = Int32.Parse(lh.getUserID());
            message.text = text;
            message.pub_date = (Int32)(DateTimeOffset.Now.ToUnixTimeSeconds());
            message.flagged = 0;

            _context.message.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Timeline");
        }

        public IActionResult Follow(Int32 author_id)
        {
            LoginHelper lh = new LoginHelper();
            Follower follower = new Follower();
            follower.who_id = Int32.Parse(lh.getUserID());
            follower.whom_id = author_id;
            _context.follower.Add(follower);
            _context.SaveChanges();

            return RedirectToAction("Timeline");
        }
        
        public IActionResult Unfollow(Int32 author_id)
        {
            LoginHelper lh = new LoginHelper();
            Follower follower = new Follower();
            follower.who_id = Int32.Parse(lh.getUserID());
            follower.whom_id = author_id;
            _context.follower.Remove(follower);
            _context.SaveChanges();

            return RedirectToAction("Timeline");
        }
        
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(User _user)
        {
            if(ModelState.IsValid)
            {
                var check = _context.user.FirstOrDefault(u => u.email == _user.email);
                if(check == null)
                {
                    GravatarImage newHash = new GravatarImage();
                    _user.pw_hash = newHash.hashBuilder(_user.pw_hash);
                    _context.user.Add(_user);
                    _context.SaveChanges();
                    return RedirectToAction("SignIn");
                }
                else
                {
                    ViewBag.error = "Email already exist";
                    return View();
                }
            }
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(string email, string pw_hash)
        {
            if(ModelState.IsValid)
            {
                GravatarImage newHash = new GravatarImage();
                var f_password = newHash.hashBuilder(pw_hash);
                var data = _context.user.Where(u => u.email.Equals(email) && u.pw_hash.Equals(f_password)).ToList();
                if(data.Count() > 0)
                {
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

                        return RedirectToAction("Timeline");
                    
                    }
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("SignIn");
                }
            }
            ViewBag.error = "Login failed";
            return View();
        }

        public async Task<IActionResult> Sign_Out()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Timeline");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
