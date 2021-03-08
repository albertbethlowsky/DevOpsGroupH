using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mvc_minitwit.Models
{
    public class ApiDataLatest
    {
        [JsonPropertyName("latest")]
        public int latest { get; set; }

    }
}