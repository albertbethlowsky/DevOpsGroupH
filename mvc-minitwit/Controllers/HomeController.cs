using System;
using System.Collections.Generic;
using System.Collections;
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
        private readonly LoginHelper lh;

        //public HomeController(ILogger<HomeController> logger, MvcDbContext context)
        public HomeController(ILogger<HomeController> logger, MvcDbContext context)

        {
            _logger = logger;
            _context = context;
            lh = new LoginHelper();
        }

        public Boolean userExistDB(){

             //if getUserid is in _context.user
            var userExist = (
                            from u in _context.user
                            select
                            new User {user_id = u.user_id, username = u.username, email = u.email, pw_hash = u.pw_hash, pw_hash2 = u.pw_hash2}
                            ).Where(u => u.user_id == lh.getUserID()).Any();
            if(userExist)
                return true;
            else
                return false;
        }

        public async Task<IActionResult> Timeline(string? id)
        {
            if(!userExistDB()){
                await Sign_Out();
            }

            if(id == lh.getUsername()) id = "My Timeline";
            if(id == "My Timeline") {
                ViewData["Title"] = "My Timeline";
                var checkfollow = (from f in _context.follower
                                    join u in _context.user on f.whom_id equals u.user_id
                                    select
                                    new Follower {who_id = f.who_id, whom_id = f.whom_id, whom_name = u.username}).Where(i => i.who_id == lh.getUserID()).ToList();
                List<Int32> followlist = new List<Int32>();
                foreach (var item in checkfollow)
                {
                    followlist.Add(item.whom_id);
                }
                followlist.Add(lh.getUserID());

                var joinedtable = (from m in _context.message
                                join u in _context.user on m.author_id equals u.user_id
                                where followlist.Contains(m.author_id)
                                select
                                new TimelineData {message_id = m.message_id, email = u.email, username = u.username, text = m.text, pub_date = m.pub_date, flagged = m.flagged})
                                                .Distinct().Where(m => m.flagged == 0).OrderByDescending(t => t.message_id).Take(50).ToList();
                return View(joinedtable);
            } else if(id == "Public Timeline" || id == null) {
                ViewData["Title"] = "Public Timeline";
                var joinedtable = (from m in _context.message
                                join u in _context.user on m.author_id equals u.user_id
                                select
                                new TimelineData {message_id = m.message_id, email = u.email, username = u.username, text = m.text, pub_date = m.pub_date, flagged = m.flagged})
                                                .Where(m => m.flagged == 0).OrderByDescending(t => t.message_id).Take(50).ToList();
                return View(joinedtable);
            } else {
                ViewData["Title"] = id + "'s Timeline";
                var joinedtable = (from m in _context.message
                                join u in _context.user on m.author_id equals u.user_id
                                where m.flagged == 0
                                select
                                new TimelineData {message_id = m.message_id, author_id = m.author_id, email = u.email, username = u.username, text = m.text, pub_date = m.pub_date, isFollowed = false})
                                                .Where(u => u.username == id).OrderByDescending(t => t.message_id).Take(50).ToList();



                if(lh.checkLogin())
                {
                    var checkfollow = (from f in _context.follower
                                    join u in _context.user on f.whom_id equals u.user_id
                                    select
                                    new Follower {who_id = f.who_id, whom_id = f.whom_id, whom_name = u.username}).Where(i => i.who_id == lh.getUserID()).ToList();

                    foreach (var item in checkfollow)
                    {
                        if(item.whom_name == id) {
                            joinedtable[0].isFollowed = true;
                        }
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
            message.author_id = lh.getUserID();
            message.text = text;
            message.pub_date = (Int32)(DateTimeOffset.Now.ToUnixTimeSeconds());
            message.flagged = 0;

            _context.message.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Timeline");
        }

        public IActionResult Follow(List<Int32> values)
        {
            Follower follower = new Follower();
            follower.who_id = values[1];
            follower.whom_id = values[0];
            _context.Add(follower);
            _context.SaveChanges();

            return RedirectToAction("Timeline");
        }

        public IActionResult Unfollow(List<Int32> values)
        {
            Follower follower = new Follower();
            follower.who_id = values[1];
            follower.whom_id = values[0];
            _context.Remove(follower);
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
            ViewData["tst"] = "tst";
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
            if(!userExistDB()){
                await Sign_Out();
            }

            if(ModelState.IsValid && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(pw_hash))
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

                        _logger.LogInformation("{userID}, just logged into the website!", user.user_id.ToString());
                        return RedirectToAction("Timeline");

                    }
                }
                else
                {
                    ViewBag.error = "Login failed";
                    ViewData["testOutput"] = "Login failed";

                    _logger.LogWarning("A user has failed to login!");
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
