using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc_minitwit.Models;
using mvc_minitwit.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace mvc_minitwit.HelperClasses
{
        public class GravatarImage{

            public GravatarImage(){}
        
        public String emailToGravatar(String email) 
        {
            email = email.Trim().ToLower();
            var emailBytes = Encoding.ASCII.GetBytes(email);
            var hashBytes = new MD5CryptoServiceProvider().ComputeHash(emailBytes);
            var hash = new StringBuilder();
            foreach (var b in hashBytes)
            {
                hash.Append(b.ToString("x2"));
            }

            var url = new StringBuilder("http://www.gravatar.com/avatar/", 48);
            url.Append(hash.ToString());
            url.Append("?d=identicon&s=48");
        
            return url.ToString();
        } 
    }
}