using System.ComponentModel.DataAnnotations;

namespace LicentaPharmastock.Models
{
    public class Location
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string address { get; set; }
    }
}
