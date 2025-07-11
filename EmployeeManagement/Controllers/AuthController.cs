using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.DTOs;
using EmployeeManagement.Services;
using System;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Invalid username or password" });

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Login exception: " + ex.Message);
                return StatusCode(500, new { error = "Server error", detail = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                if (!result)
                    return BadRequest(new { message = "Username or email already exists" });

                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Register exception: " + ex.Message);
                return StatusCode(500, new { error = "Server error", detail = ex.Message });
            }
        }
    }
}
