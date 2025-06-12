using System.ComponentModel.DataAnnotations;

namespace LicentaPharmastock.Models
{
    public class ManagerPassword
    {
        public int Id { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}
