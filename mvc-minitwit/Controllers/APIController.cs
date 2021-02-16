using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc_minitwit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        

        // https://localhost:5001/api
        [HttpGet]
        [Route("/api/")]
        public IActionResult GetAllCities()
        {
            return Ok("hej");       //Ok returns in JSON-format
        }

        [HttpGet("/api/{startCh}")]
        [Route("/api/latest")]
        public IActionResult GetLatest()
        {
            return Ok("hej");       //Ok returns in JSON-format
        }

    }
}
