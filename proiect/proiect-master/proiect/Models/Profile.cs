using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace proiect.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }

        [ForeignKey("ApplicationUser")]
        public String ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        
        [MaxLength(255, ErrorMessage = "Description can not have more than 255 characters")]
        public String Description { get; set; }

        [MaxLength(20, ErrorMessage = "Name can not have more than 20 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only have letters.")]
        public string Name { get; set; }

     
    }
}