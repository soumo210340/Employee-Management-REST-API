using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;

namespace EmployeeManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly EmployeeDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(EmployeeDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            Console.WriteLine($"🔍 Attempting login for user: {loginDto.Username}");
            
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == loginDto.Username);
            
            if (employee == null)
            {
                Console.WriteLine($"❌ User not found: {loginDto.Username}");
                return string.Empty;
            }
            
            if (!employee.IsActive)
            {
                Console.WriteLine($"❌ User account is not active: {loginDto.Username}");
                return string.Empty;
            }
            
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, employee.PasswordHash);
            if (!isPasswordValid)
            {
                Console.WriteLine($"❌ Invalid password for user: {loginDto.Username}");
                return string.Empty;
            }
            
            Console.WriteLine($"✅ Valid credentials for user: {loginDto.Username}");
            var token = GenerateJwtToken(employee);
            Console.WriteLine($"🔑 Generated JWT token for user: {loginDto.Username}");
            return token;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Employees.AnyAsync(e => e.Username == registerDto.Username || e.Email == registerDto.Email))
                return false;
            var employee = new Employee
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = string.Empty,
                LastName = string.Empty,
                Phone = string.Empty,
                Department = string.Empty,
                Position = string.Empty,
                Salary = 0,
                HireDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                Role = registerDto.Role ?? "User"
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(Employee employee)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                throw new InvalidOperationException("JWT configuration is missing required values.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name, employee.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, employee.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Use UtcNow for token expiration
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
