using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeDbContext _context;

        public EmployeeService(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Department = e.Department,
                    Position = e.Position,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var e = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            if (e == null) return null;
            return new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                Department = e.Department,
                Position = e.Position,
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsActive = e.IsActive
            };
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("First name and last name are required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Phone))
                throw new ArgumentException("Phone number is required.");

            if (string.IsNullOrWhiteSpace(dto.Department))
                throw new ArgumentException("Department is required.");

            if (string.IsNullOrWhiteSpace(dto.Position))
                throw new ArgumentException("Position is required.");

            if (dto.Salary <= 0)
                throw new ArgumentException("Salary must be greater than zero.");

            var normalizedEmail = dto.Email.Trim();
            if (await _context.Employees.AnyAsync(x => x.Email == normalizedEmail))
                throw new ArgumentException("Email already exists.");

            var baseUsername = $"{dto.FirstName}.{dto.LastName}"
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", string.Empty);
            if (string.IsNullOrWhiteSpace(baseUsername))
            {
                baseUsername = $"employee{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            }

            var e = new Employee
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = normalizedEmail,
                Phone = dto.Phone.Trim(),
                Department = dto.Department.Trim(),
                Position = dto.Position.Trim(),
                Salary = dto.Salary,
                HireDate = dto.HireDate,
                Username = await GenerateUniqueUsernameAsync(baseUsername)
            };

            _context.Employees.Add(e);
            await _context.SaveChangesAsync();

            return new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                Department = e.Department,
                Position = e.Position,
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsActive = e.IsActive
            };
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, CreateEmployeeDto dto)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null || !e.IsActive) return null;

            var normalizedEmail = dto.Email.Trim();
            if (await _context.Employees.AnyAsync(x => x.Id != id && x.Email == normalizedEmail))
                throw new ArgumentException("Email already exists.");

            e.FirstName = dto.FirstName.Trim();
            e.LastName = dto.LastName.Trim();
            e.Email = normalizedEmail;
            e.Phone = dto.Phone.Trim();
            e.Department = dto.Department.Trim();
            e.Position = dto.Position.Trim();
            e.Salary = dto.Salary;
            e.HireDate = dto.HireDate;
            e.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                Department = e.Department,
                Position = e.Position,
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsActive = e.IsActive
            };
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null || !e.IsActive) return false;
            e.IsActive = false;
            e.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EmployeeDto> RegisterEmployeeAsync(RegisterEmployeeDto dto)
        {
            // Check for existing username/email in Employees
            if (await _context.Employees.AnyAsync(e => e.Username == dto.Username || e.Email == dto.Email))
                throw new System.Exception("Username or email already exists.");

            // Create Employee
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department,
                Position = dto.Position,
                Salary = dto.Salary,
                HireDate = dto.HireDate,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow,
                IsActive = true
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Department = employee.Department,
                Position = employee.Position,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive
            };
        }

        private async Task<string> GenerateUniqueUsernameAsync(string baseUsername)
        {
            var candidate = baseUsername;
            var suffix = 1;

            while (await _context.Employees.AnyAsync(e => e.Username == candidate))
            {
                candidate = $"{baseUsername}{suffix}";
                suffix++;
            }

            return candidate;
        }
    }
}
