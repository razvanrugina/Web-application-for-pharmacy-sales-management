using System.ComponentModel.DataAnnotations;
namespace LicentaPharmastock.Models
{
    public class Brand
    {
        public int Id { get; set; }   // Primary Key (immutable after creation)
        [Required]
        public string name { get; set; }
        public Brand()
        {

        }
    }
}
