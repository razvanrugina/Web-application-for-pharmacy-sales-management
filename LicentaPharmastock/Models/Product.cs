using LicentaPharmastock.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace LicentaPharmastock.Models
{
    public class Product
    {
        public int Id { get;  set; }  // Primary Key (immutable after creation)

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public ProductType Type { get; set; }

        [Required]
        public int BrandId { get; set; }     // Foreign Key

        public Brand? Brand { get; set; }    // Navigation Property


        [MaxLength(2000)]
        public string Prospectus { get; set; }

        public string ImagePath { get; set; }

        public List<string> Locations { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }



        public Product()
        {
            
        }
    }
}
