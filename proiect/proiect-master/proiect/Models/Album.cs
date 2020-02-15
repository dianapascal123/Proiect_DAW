using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiect.Models
{
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }

        [Required(ErrorMessage = "Add a title")]
        [MaxLength(20, ErrorMessage = "Title can`t have more than 20 characters")]
        [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "Title can only have letters.")]
        public string AlbumTitle { get; set; }


        //chei externe
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        


        public virtual ICollection<Photo> Photo { get; set; }
    }
}