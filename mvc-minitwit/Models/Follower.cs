using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvc_minitwit.Models {

    public class Follower {
    
    [Key][Column(Order=0)]
    public int who_id { get; set; }
    [Key][Column(Order=1)]
    public int whom_id { get; set; }

    [NotMapped]
    public string whom_name { get; set; }
    }

}