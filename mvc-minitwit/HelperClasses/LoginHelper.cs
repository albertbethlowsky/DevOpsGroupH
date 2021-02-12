using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;


namespace mvc_minitwit.HelperClasses
{
        public class LoginHelper {

            public LoginHelper(){}

            public Boolean checkLogin()
            {
                HttpContextAccessor accessor = new HttpContextAccessor();
                var cookie = accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
                if(cookie != null)
                {
                    return true;
                } else {
                    return false;
                }
            }
        }
}