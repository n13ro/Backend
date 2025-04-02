
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
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody]RegisterDto registerDto)
        {
            if (_dbContext.Users.Any(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { mess = "User already exists" });
            }

            var user = new User
            {
                Email = registerDto.Email,
                Passwd = HashPasswd(registerDto.Password),
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            //var token = _jwtService.GenerateToken(user);
            //return new AuthResponseDto { Token = token };
            GC.Collect();
            return Ok(new { mess = "User is created" });
            }

        [HttpPost("login")]
        public ActionResult<AuthResponseDto> Login([FromBody] LoginDto loginDto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginDto.Email);
            var hPs = HashPasswd(loginDto.Password);

            if (user == null || !(user.Passwd == hPs))
            {
                return Unauthorized(new { mess = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(user, loginDto.RememberMe);
            
            return new AuthResponseDto { Token = token };

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
