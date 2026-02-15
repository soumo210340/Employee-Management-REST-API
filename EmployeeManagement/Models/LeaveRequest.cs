using System;

namespace EmployeeManagement.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
