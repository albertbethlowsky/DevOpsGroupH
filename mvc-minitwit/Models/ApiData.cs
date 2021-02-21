using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mvc_minitwit.Models
{
    public class ApiData
    {
        [JsonIgnore]
        [JsonPropertyName("follow")]
        public string follow { get; set; }

        [JsonIgnore]
        [JsonPropertyName("unfollow")]
        public string unfollow { get; set; }

        [JsonPropertyName("latest")]
        public int latest { get; set; }
    }
}