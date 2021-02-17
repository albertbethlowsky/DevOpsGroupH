using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace mvc_minitwit.Models
{
    public class ApiData
    {
        [JsonProperty(PropertyName = "follow")]
        public string follow { get; set; }
        [JsonProperty(PropertyName = "unfollow")]
        public string unfollow { get; set; }
    }
}