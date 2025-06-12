using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LicentaPharmastock.Models.Enums;

namespace LicentaPharmastock.Models
{
    public class Product
    {
        public int Id { get; set; }  // Primary Key

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public ProductType Type { get; set; }

        [Required]
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        [Required]
        [Display(Name = "Units per Package")]
        public int UnitsPerPackage { get; set; }

        [Required]
        [Display(Name = "Full Packages")]
        public int PackageCount { get; set; }

        [Display(Name = "Loose Units")]
        public int LooseUnitCount { get; set; } = 0;

        [NotMapped]
        [Display(Name = "Total Units")]
        public int Quantity => PackageCount * UnitsPerPackage + LooseUnitCount;

        [Required]
        public List<Location> Locations { get; set; } = new List<Location>();

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [Display(Name = "Package Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PackagePrice { get; set; }

        [NotMapped]
        public decimal UnitPrice => PackagePrice / UnitsPerPackage;
    }
}
