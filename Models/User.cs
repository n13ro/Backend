//using Backend.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        [Key] public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Passwd { get; set; }
        public string Role { get; set; } = "User";

        public List<Product> Products { get; set; } = new List<Product>();
}

}
