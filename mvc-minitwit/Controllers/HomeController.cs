using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Timeline(int? id)
        {         
            //List<Message> messages = _context.message.OrderByDescending(t => t.message_id).Take(50).ToList();
            //List<User> users = _context.user.ToList();
            var joinedtable = (from m in _context.message
                              join u in _context.user on m.author_id equals u.user_id
                              select 
                                new TimelineData {message_id = m.message_id, email = u.email, username = u.username, text = m.text, pub_date = m.pub_date}).OrderByDescending(t => t.message_id).Take(50).ToList();

            return View(joinedtable);
        }

        public IActionResult UserTimeline()
        {
            return View();
        }

        public IActionResult MyTimeline()
        {
            return View();
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
                var check = _context.user.FirstOrDefault(t => t.email == _user.email);
                if(check == null)
                {
                    GravatarImage newHash = new GravatarImage();
                    _user.pw_hash = newHash.hashBuilder(_user.pw_hash);
                    _context.user.Add(_user);
                    _context.SaveChanges();
                    return RedirectToAction("Timeline");
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
        public IActionResult SignIn(User _user)
        {
            if(ModelState.IsValid)
            {
                GravatarImage newHash = new GravatarImage();
                var f_password = newHash.hashBuilder(_user.pw_hash);
                var data = _context.user.Where(u => u.email.Equals(_user.email) && u.pw_hash.Equals(f_password)).ToList();
                
                if(data.Count() > 0)
                {
                    return RedirectToAction("Timeline");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Timeline");
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
