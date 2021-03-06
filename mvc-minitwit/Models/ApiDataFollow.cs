using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace mvc_minitwit.Models
{
    public class ApiDataFollow
    {
        [JsonIgnore]
        [JsonPropertyName("follow")]
        public string follow { get; set; }

        [JsonIgnore]
        [JsonPropertyName("unfollow")]
        public string unfollow { get; set; }
    }
}
