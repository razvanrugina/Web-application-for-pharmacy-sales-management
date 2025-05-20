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
        [Required]
        public string ImagePath { get; set; }
        [Required]
        public List<string> Locations { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        [Display(Name = "Price")]
        public float price { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int quantity { get; set; }

        public Product()
        {
            
        }
    }
}
