using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Services;
using EmployeeManagement.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Controllers
{
    // Handles authentication endpoints
    // /api/auth/login - User login
    // /api/auth/register - User registration
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authService.LoginAsync(loginDto);
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { Message = "Invalid username or password." });
            return Ok(new { Token = token });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(registerDto);
            if (!result)
                return BadRequest(new { Message = "Username or email already exists." });
            return Ok(new { Message = "Registration successful." });
        }
    }
}
