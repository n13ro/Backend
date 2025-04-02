//using Backend.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
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

        public string NickName { get; set; } = "";
        public string FirstName { get; set; } = "";

        public string SurName { get; set; } = "";

        public string Address { get; set; } = "";

        //public List<Product> Products { get; set; } = new List<Product>();
    }

}
