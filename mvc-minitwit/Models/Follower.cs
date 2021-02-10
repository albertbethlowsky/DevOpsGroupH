using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvc_minitwit.Models {

    public class Follower {
   [Key] //TODO!: WE NEED TO FIND A WAY TO SET IT TO KEYLESS!! 
    public int who_id { get; set; }
    public int whom_id { get; set; }

    
    }

}