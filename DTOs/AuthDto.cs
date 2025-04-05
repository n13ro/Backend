using Castle.Components.DictionaryAdapter;
using System.ComponentModel;
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
            [MinLength(8, ErrorMessage = "Пароль должен быть больше 8 символов")]
            public required string Password { get; set; }
            
            [Required]
            [MinLength(1, ErrorMessage = "Минмум 1 символ")]
            public required string NickName { get; set; }

            [Required]
            [MinLength(1, ErrorMessage = "Минмум 1 символ")]
            public required string FirstName { get; set; }
            [Required]
            [MinLength(1, ErrorMessage = "Минмум 1 символ")]
            public required string SurName { get; set; }

        }
        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [MinLength(8, ErrorMessage = "Пароль должен быть больше 8 символов")]
            public required string Password { get; set; }
            
            public bool RememberMe { get; set; }
        }
        public class AuthResponseDto
        {
            public required string Token { get; set; }
        }
    }
}
