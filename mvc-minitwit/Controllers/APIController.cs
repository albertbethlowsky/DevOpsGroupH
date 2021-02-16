using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Api;
using mvc_minitwit.Data;
using mvc_minitwit.Models;

namespace mvc_minitwit.Controllers
{
    //localhost:<portnr>/API/msgs
    [Route("[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly MvcDbContext _context;

        public APIController(MvcDbContext context)
        {
            _context = context;
        }

        [HttpGet("{msgs}")] //https://localhost:5001/msgs
        public async Task<ActionResult<IEnumerable<Message>>> Getmessage()
        {
            return await _context.message.OrderByDescending(m => m.pub_date)
                .Take(100)
                .ToListAsync();
        }
    }
}
