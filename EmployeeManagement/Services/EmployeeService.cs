using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
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
            var e = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department,
                Position = dto.Position,
                Salary = dto.Salary,
                HireDate = dto.HireDate
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
            e.FirstName = dto.FirstName;
            e.LastName = dto.LastName;
            e.Email = dto.Email;
            e.Phone = dto.Phone;
            e.Department = dto.Department;
            e.Position = dto.Position;
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
            // Check for existing username/email in Users
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                throw new System.Exception("Username or email already exists.");
            // Check for existing email in Employees
            if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
                throw new System.Exception("Employee email already exists.");

            // Create User
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

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
                HireDate = dto.HireDate
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
    }
}
