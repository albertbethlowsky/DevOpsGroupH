using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mvc_minitwit.Models
{
    public class ApiDataFollows
    {
        [JsonPropertyName("follows")]
        public List<string> follows { get; set; }

    }
}
