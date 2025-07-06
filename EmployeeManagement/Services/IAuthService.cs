
using EmployeeManagement.DTOs;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(RegisterDto registerDto);
    }
}
