using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiect.Models
{
    public class Photo
    {
        [Key]
        public int PhotoId { get; set; }
        /*[Required(ErrorMessage = "Upload a photo")]
        public string Location { get; set; }*/

        [Required(ErrorMessage = "Add a description")]
        [MaxLength(100, ErrorMessage = "Description can`t have more than 100 characters")]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        /*[NotMapped]*/
        public string PhotoFile { get; set; }



        //chei externe
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public int AlbumId { get; set; }

        public virtual Category Category { get; set; } //datele despre categoria din care face parte
        public IEnumerable<SelectListItem> Categories { get; set; } //doar pt drop down
        public virtual ApplicationUser User { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }

        public virtual Album Album { get; set; }

        public IEnumerable<SelectListItem> Albums { get; set; }
    }
}