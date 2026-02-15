using EmployeeManagement.DTOs;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveService;
        public LeaveRequestsController(ILeaveRequestService leaveService)
        {
            _leaveService = leaveService;
        }

        // Admin: Get all leave requests
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var leaves = await _leaveService.GetAllAsync();
            return Ok(leaves);
        }

        // Employee: Get their own leave requests
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetMy()
        {
            if (!TryGetCurrentUserId(out var empId))
                return Unauthorized(new { message = "Invalid or missing user ID claim." });

            var leaves = await _leaveService.GetByEmployeeIdAsync(empId);
            return Ok(leaves);
        }

        // Employee: Apply for leave
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Apply([FromBody] LeaveRequestDto dto)
        {
            if (!TryGetCurrentUserId(out var empId))
                return Unauthorized(new { message = "Invalid or missing user ID claim." });

            dto.EmployeeId = empId;
            var created = await _leaveService.CreateAsync(dto);
            return Ok(created);
        }

        // Admin: Approve leave
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            if (!TryGetCurrentUserId(out var adminId))
                return Unauthorized(new { message = "Invalid or missing user ID claim." });

            var result = await _leaveService.ApproveAsync(id, adminId);
            return result ? Ok() : BadRequest();
        }

        // Admin: Reject leave
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            if (!TryGetCurrentUserId(out var adminId))
                return Unauthorized(new { message = "Invalid or missing user ID claim." });

            var result = await _leaveService.RejectAsync(id, adminId);
            return result ? Ok() : BadRequest();
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out userId);
        }
    }
}
