using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Database;
using Backend.Models;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using static Backend.DTOs.AuthDto;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowedHosts")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly JwtService _jwtService;
        
        // Использование словаря для хранения refresh токенов в памяти (userId -> refreshToken)
        private static readonly HashSet<(Guid userId, string token)> _refreshTokens = new();
        
        public AuthController(AppDbContext dbContext, JwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        private static string HashPasswd(string str)
        {
            var hashedBytesSha256 = SHA256.HashData(Encoding.UTF8.GetBytes(str));
            return Convert.ToBase64String(hashedBytesSha256);
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponse>> Register([FromBody]RegisterDto registerDto)
        {
            if (_dbContext.Users.Any(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { mess = "User already exists" });
            }

            var user = new User
            {
                Email = registerDto.Email,
                Passwd = HashPasswd(registerDto.Password),
                NickName = registerDto.NickName,
                FirstName = registerDto.FirstName,
                SurName = registerDto.SurName,
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            
            // Генерация access токена
            var accessToken = _jwtService.GenerateToken(user);
            
            // Генерация refresh токена
            var refreshToken = _jwtService.GenerateRefreshToken();
            _refreshTokens.Add((user.Id, refreshToken));
            
            // Возвращаем оба токена
            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        [HttpPost("login")]
        public ActionResult<TokenResponse> Login([FromBody] LoginDto loginDto)
        {
            // Поиск пользователя по email
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginDto.Email);
            
            // Хеширование введенного пароля для сравнения
            var hPs = HashPasswd(loginDto.Password);

            // Проверка учетных данных
            if (user == null || !(user.Passwd == hPs))
            {
                return Unauthorized(new { mess = "Invalid credentials" });
            }

            // Генерация access токена
            var accessToken = _jwtService.GenerateToken(user, loginDto.RememberMe);
            
            // Генерация refresh токена
            var refreshToken = _jwtService.GenerateRefreshToken();
            _refreshTokens.Add((user.Id, refreshToken));
            
            // Возвращаем оба токена
            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        [Authorize]
        [HttpPost("refresh")]
        public ActionResult<TokenResponse> Refresh([FromBody] RefreshTokenRequest request)
        {
            // Проверяем, что refresh токен не пустой
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "Refresh токен не может быть пустым" });
            }
            
            // Получаем access токен из заголовка Authorization
            var accessToken = HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?
                .Replace("Bearer ", "")
                .Trim();
            
            // Проверяем, что access токен предоставлен
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest(new { message = "Необходимо предоставить Access токен в заголовке Authorization" });
            }
            
            try
            {
                // Извлекаем информацию из существующего токена, даже если он истек
                var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
                
                // Получаем ID пользователя из токена
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return BadRequest(new { message = "Невозможно определить пользователя из токена" });
                }
                var userId = Guid.Parse(userIdClaim.Value);
                
                // Проверяем, что у нас есть сохраненный refresh токен для этого пользователя
                var storedToken = _refreshTokens.FirstOrDefault(t => t.userId == userId);
                if (storedToken == default)
                {
                    return BadRequest(new { message = "Refresh токен не найден" });
                }
                
                if (storedToken.token != request.RefreshToken)
                {
                    return BadRequest(new { message = "Недействительный refresh токен" });
                }
                
                // Находим пользователя в базе данных
                var user = _dbContext.Users.Find(userId);
                if (user == null)
                {
                    return BadRequest(new { message = "Пользователь не найден" });
                }
                
                // Получаем настройку "запомнить меня" из исходного токена
                var rememberMe = principal.Claims
                    .FirstOrDefault(c => c.Type == "RememberMe")?.Value == "true";
                
                // Создаем новый access токен
                var newAccessToken = _jwtService.GenerateToken(user, rememberMe);
                
                // Создаем новый refresh токен
                var newRefreshToken = _jwtService.GenerateRefreshToken();
                
                // Сохраняем новый refresh токен
                _refreshTokens.RemoveWhere(t => t.userId == userId);
                _refreshTokens.Add((userId, newRefreshToken));
                
                // Возвращаем новую пару токенов
                return new TokenResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при обработке токена", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("updateUser/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody]UpdateUserDto updateUser)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var currentUserId = Guid.Parse(userIdClaim.Value);

            if (currentUserId != id)
            {
                return Forbid();
            }

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            bool state = false;

            if (!string.IsNullOrWhiteSpace(updateUser.NickName))
            {
                user.NickName = updateUser.NickName;
                state = true;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.FirstName))
            {
                user.FirstName = updateUser.FirstName;
                state = true;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.SurName))
            {
                user.SurName = updateUser.SurName;
                state = true;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.Address))
            {
                user.Address = updateUser.Address;
                state = true;
            }

            if (state)
            {
                try
                {
                    await _dbContext.SaveChangesAsync();
                    return Ok(new { mess = "Успешно!!!" });
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { mess = "Произошла ошибка при изменении данных.", exception = ex.Message });
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { mess = "Пользователя нет" });
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            GC.Collect();
            return Ok(new { mess = "Пользователь удален!" });
        } 
    }
}
