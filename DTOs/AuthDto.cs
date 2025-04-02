using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class AuthDto
    {
        public class RegisterDto
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public required string Password { get; set; }
        }
        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public required string Password { get; set; }
            
            public bool RememberMe { get; set; }
        }
        public class AuthResponseDto
        {
            public required string Token { get; set; }
        }
    }
}
