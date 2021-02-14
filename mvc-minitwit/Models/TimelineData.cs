using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mvc_minitwit.Models{
    public class TimelineData {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int message_id {get; set;} //autoincremet
        [Required]
        public int author_id {get;set;}
        [Required]
        public string text {get; set;}
        public int pub_date {get;set;}
        public int flagged {get;set;}
        public int user_id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string pw_hash { get; set; }
        public int who_id { get; set; }
        public int whom_id { get; set; }

        [NotMapped]
        public bool isFollowed { get; set; }
    }

 }