using System;
using System.ComponentModel.DataAnnotations;

namespace mvc_minitwit.Models{
    public class Message {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int message_id {get; set;} //autoincremet
        public int author_id {get;set;}
        public string text {get; set;}
        public int pub_date {get;set;}
        public int falgged {get;set;}
    }

    // public class Follower {
    //     public int who_id {get; set;}
    //     public int whom_id {get; set;}
    // }

    // public class User {
    //     [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //     public int user_id{get; set;}
    //     public string username {get; set;}
    //     public string email {get; set;}
    //     string pwd_hash {get; set;}
    // }
}

