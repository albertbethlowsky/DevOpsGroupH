using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mvc_minitwit.Models{
    public class Message {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int message_id {get; set;} //autoincremet
        [Required]
        public int author_id {get;set;}
        public User author { get; set; }
        [Required]
        public string text {get; set;}
        public int pub_date {get;set;}
        public int flagged {get;set;}
    }

 }