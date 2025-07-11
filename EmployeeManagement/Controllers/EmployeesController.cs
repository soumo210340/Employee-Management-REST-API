using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EmployeeManagement.Services;
using EmployeeManagement.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all employees.
        /// </summary>
        /// <returns>List of employees.</returns>
        // GET: api/employees
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees");
                return StatusCode(500, new { Message = "An error occurred while retrieving employees.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific employee by ID.
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details or 404 if not found.</returns>
        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound(new { Message = "Employee not found" });
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving employee with ID {id}");
                return StatusCode(500, new { Message = "An error occurred while retrieving the employee.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="createEmployeeDto">Employee data</param>
        /// <returns>The created employee.</returns>
        // POST: api/employees
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var employee = await _employeeService.CreateEmployeeAsync(createEmployeeDto);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while creating employee");
                return StatusCode(500, new { Message = "A database error occurred while creating the employee.", Details = dbEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, new { Message = "An error occurred while creating the employee.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing employee.
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="updateEmployeeDto">Updated employee data</param>
        /// <returns>The updated employee or 404 if not found.</returns>
        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] CreateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var employee = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);
                if (employee == null)
                    return NotFound(new { Message = "Employee not found" });
                return Ok(employee);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error while updating employee with ID {id}");
                return StatusCode(500, new { Message = "A database error occurred while updating the employee.", Details = dbEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating employee with ID {id}");
                return StatusCode(500, new { Message = "An error occurred while updating the employee.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an employee by ID.
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>No content if successful, 404 if not found.</returns>
        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var result = await _employeeService.DeleteEmployeeAsync(id);
                if (!result)
                    return NotFound(new { Message = "Employee not found" });
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error while deleting employee with ID {id}");
                return StatusCode(500, new { Message = "A database error occurred while deleting the employee.", Details = dbEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {id}");
                return StatusCode(500, new { Message = "An error occurred while deleting the employee.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Registers a new employee and creates a login for them.
        /// </summary>
        /// <param name="registerEmployeeDto">Employee registration data</param>
        /// <returns>The created employee.</returns>
        // POST: api/employees/register
        [HttpPost("register")]
        [AllowAnonymous] // Allow public registration
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto registerEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var employee = await _employeeService.RegisterEmployeeAsync(registerEmployeeDto);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error registering employee");
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
