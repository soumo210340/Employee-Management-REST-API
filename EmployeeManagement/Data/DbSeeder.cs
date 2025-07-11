// Database seeding logic
// Default admin user creation
// Sample employee data

using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace EmployeeManagement.Data
{
    public static class DbSeeder
    {
        public static void Seed(EmployeeDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();
            // No Users seeding needed
        }
    }
}
