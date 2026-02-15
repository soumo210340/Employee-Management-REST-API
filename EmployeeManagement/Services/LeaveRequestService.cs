using EmployeeManagement.DTOs;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly EmployeeDbContext _context;
        public LeaveRequestService(EmployeeDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LeaveRequestDto>> GetAllAsync()
        {
            return await _context.LeaveRequests.Select(l => new LeaveRequestDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status,
                RequestedAt = l.RequestedAt,
                ReviewedBy = l.ReviewedBy,
                ReviewedAt = l.ReviewedAt
            }).ToListAsync();
        }
        public async Task<IEnumerable<LeaveRequestDto>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests.Where(l => l.EmployeeId == employeeId).Select(l => new LeaveRequestDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status,
                RequestedAt = l.RequestedAt,
                ReviewedBy = l.ReviewedBy,
                ReviewedAt = l.ReviewedAt
            }).ToListAsync();
        }
        public async Task<LeaveRequestDto?> GetByIdAsync(int id)
        {
            var l = await _context.LeaveRequests.FindAsync(id);
            if (l == null) return null;
            return new LeaveRequestDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status,
                RequestedAt = l.RequestedAt,
                ReviewedBy = l.ReviewedBy,
                ReviewedAt = l.ReviewedAt
            };
        }
        public async Task<LeaveRequestDto> CreateAsync(LeaveRequestDto dto)
        {
            var entity = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };
            _context.LeaveRequests.Add(entity);
            await _context.SaveChangesAsync();
            dto.Id = entity.Id;
            dto.Status = entity.Status;
            dto.RequestedAt = entity.RequestedAt;
            return dto;
        }
        public async Task<bool> ApproveAsync(int id, int adminId)
        {
            var l = await _context.LeaveRequests.FindAsync(id);
            if (l == null || l.Status != "Pending") return false;
            l.Status = "Approved";
            l.ReviewedBy = adminId;
            l.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RejectAsync(int id, int adminId)
        {
            var l = await _context.LeaveRequests.FindAsync(id);
            if (l == null || l.Status != "Pending") return false;
            l.Status = "Rejected";
            l.ReviewedBy = adminId;
            l.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
