using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_minitwit.Models {

    public class User {

        [Key]
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string pw_hash { get; set; }
        [Required]
        [NotMapped]
        [System.ComponentModel.DataAnnotations.Compare("pw_hash")]

        public string pw_hash2 { get; set; }
    }
}