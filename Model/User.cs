﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guestbook.Model
{
    public class User
    {
        
        [Key]
        public int User_Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime Date_of_Birth { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        public string Password { get; set; }
        
    }
}
