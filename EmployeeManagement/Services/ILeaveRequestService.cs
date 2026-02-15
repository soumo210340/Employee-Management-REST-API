using EmployeeManagement.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestDto>> GetAllAsync();
        Task<IEnumerable<LeaveRequestDto>> GetByEmployeeIdAsync(int employeeId);
        Task<LeaveRequestDto?> GetByIdAsync(int id);
        Task<LeaveRequestDto> CreateAsync(LeaveRequestDto dto);
        Task<bool> ApproveAsync(int id, int adminId);
        Task<bool> RejectAsync(int id, int adminId);
    }
}
