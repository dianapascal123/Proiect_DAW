using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiect.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }


        [Required(ErrorMessage = "Add a comment")]
        [MaxLength(255, ErrorMessage = "Comment can`t have more than 255 characters")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        //chei externe
        public int PhotoId { get; set; }
        public string UserId { get; set; }

        public virtual Photo Photo { get; set; }
        public IEnumerable<SelectListItem> Photos { get; set; }

        public virtual ApplicationUser User { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }

    }
}