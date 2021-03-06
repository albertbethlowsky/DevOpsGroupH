using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace mvc_minitwit.Models
{
    public class ApiDataFollow
    {
        [JsonPropertyName("follow")]
        public string follow { get; set; }

        [JsonPropertyName("unfollow")]
        public string unfollow { get; set; }
    }
}
