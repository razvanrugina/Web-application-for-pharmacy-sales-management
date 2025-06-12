using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace LicentaPharmastock.Models
{
    public class Brand
    {
        public int Id { get; set; }   // Primary Key (immutable after creation)
        [Required]
        [Remote(action: "VerifyName", controller: "Brands", ErrorMessage = "Name already taken.")]
        public string name { get; set; }
        public Brand()
        {

        }
    }
}
