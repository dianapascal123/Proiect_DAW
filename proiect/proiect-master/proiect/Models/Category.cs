using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace proiect.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Add a category")]
        [MaxLength(20, ErrorMessage = "Name can`t have more than 20 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Title can only have letters.")]
        public string CategoryName { get; set; }

        public virtual ICollection<Photo> Photo { get; set; } //parte din setarea one to many
    }
}