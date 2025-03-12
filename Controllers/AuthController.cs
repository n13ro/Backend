
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using backend.Database;
using backend.Models;
using Backend.Enums;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static Backend.DTOs.AuthDto;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<AuthResponseDto>> Register([FromForm]RegisterDto registerDto)
        {
            if (_dbContext.Users.Any(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { mess = "User already exists" });
            }

            var user = new User
            {
                Email = registerDto.Email,
                Passwd = HashPasswd(registerDto.Password),
                Role = "User"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);
            return new AuthResponseDto { Token = token };
            //return Ok(new { mess = "User register" });
            }

        [HttpPost("login")]
        public ActionResult<AuthResponseDto> Login([FromForm]LoginDto loginDto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginDto.Email);
            var hPs = HashPasswd(loginDto.Password);
            if (user == null || !(user.Passwd == hPs))
            {
                return Unauthorized(new { mess = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(user);
            
            return new AuthResponseDto { Token = token };
        }

    }
    
}
