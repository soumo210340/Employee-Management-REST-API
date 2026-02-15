// Simple frontend logic for login, registration, and employee management
const apiBase = "https://localhost:5001/api"; // ✅ must use https!
let jwtToken = "eyJhbGciOiJIUzI1NiIs..."; // Temporary hardcoded token for testing

function showRegister() {
    document.getElementById('login-form').style.display = 'none';
    document.getElementById('register-form').style.display = 'block';
}
function showLogin() {
    document.getElementById('register-form').style.display = 'none';
    document.getElementById('login-form').style.display = 'block';
}
function showMain() {
    document.getElementById('auth-section').style.display = 'none';
    document.getElementById('main-section').style.display = 'block';
    loadEmployees();
}
function logout() {
    jwtToken = null;
    document.getElementById('main-section').style.display = 'none';
    document.getElementById('auth-section').style.display = 'block';
}
async function login() {
    const username = document.getElementById('login-username').value;
    const password = document.getElementById('login-password').value;
    const errorDiv = document.getElementById('login-error');
    errorDiv.textContent = '';
    try {
        const res = await fetch(`${apiBase}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        if (!res.ok) {
            const errorText = await res.text();
            console.error('Login failed:', errorText);
            throw new Error('Login failed');
        }
        const data = await res.json();
        jwtToken = data.token;
        console.log("Logged in with token:", jwtToken);
        showMain();
    } catch (err) {
        alert('Login failed: Invalid username or password.');
        errorDiv.textContent = 'Invalid username or password.';
    }
}
async function register() {
    const username = document.getElementById('register-username').value;
    const password = document.getElementById('register-password').value;
    const errorDiv = document.getElementById('register-error');
    errorDiv.textContent = '';
    try {
        const res = await fetch(`${apiBase}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        if (!res.ok) {
            const errorText = await res.text();
            console.error('Registration failed:', errorText);
            throw new Error('Registration failed');
        }
        showLogin();
    } catch (err) {
        errorDiv.textContent = 'Registration failed. Try a different username.';
    }
}
document.getElementById('add-employee-form').addEventListener('submit', async function(e) {
    e.preventDefault();
    const firstName = document.getElementById('emp-firstname').value;
    const lastName = document.getElementById('emp-lastname').value;
    const email = document.getElementById('emp-email').value;
    const department = document.getElementById('emp-department').value;
    const position = document.getElementById('emp-position').value;
    const role = document.getElementById('emp-role').value; // Get the selected role
    const errorDiv = document.getElementById('employee-error');
    errorDiv.textContent = '';
    try {
        const res = await fetch(`${apiBase}/employees`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwtToken}`
            },
            body: JSON.stringify({ firstName, lastName, email, department, position, role }) // Include role in the request
        });
        if (!res.ok) throw new Error('Add failed');
        loadEmployees();
        this.reset();
    } catch (err) {
        errorDiv.textContent = 'Failed to add employee.';
    }
});
// Utility function to check if JWT token is valid
function isTokenValid(token) {
    if (!token) return false;
    const payload = JSON.parse(atob(token.split('.')[1]));
    const now = Math.floor(Date.now() / 1000);
    return payload.exp > now;
}

// Ensure token validity before making API calls
async function fetchWithAuth(url, options = {}) {
    if (!isTokenValid(jwtToken)) {
        alert('Session expired. Please log in again.');
        logout();
        return;
    }
    options.headers = {
        ...options.headers,
        'Authorization': `Bearer ${jwtToken}`
    };
    const response = await fetch(url, options);
    if (!response.ok) {
        const errorText = await response.text();
        console.error(`Error: ${response.status} - ${errorText}`);
        throw new Error(errorText);
    }
    return response;
}

// Update existing API calls to correctly map backend fields
async function loadEmployees() {
    const tbody = document.querySelector('#employee-table tbody');
    tbody.innerHTML = '';
    try {
        const res = await fetchWithAuth(`${apiBase}/employees`);
        const employees = await res.json();
        console.log('API Response:', employees); // Debugging log to capture API response
        employees.forEach(emp => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${emp.Id}</td> <!-- Ensure field names match API response -->
                <td>${emp.FirstName} ${emp.LastName}</td>
                <td>${emp.Email}</td>
                <td>${emp.Department}</td>
                <td>${emp.Position}</td>
                <td><button onclick="deleteEmployee(${emp.Id})">Delete</button></td>
            `;
            tbody.appendChild(tr);
        });
    } catch (err) {
        console.error('Error loading employees:', err); // Debugging log for errors
        tbody.innerHTML = '<tr><td colspan="6">Failed to load employees.</td></tr>';
    }
}

async function deleteEmployee(id) {
    if (!confirm('Delete this employee?')) return;
    try {
        await fetchWithAuth(`${apiBase}/employees/${id}`, { method: 'DELETE' });
        loadEmployees();
    } catch (err) {
        alert('Failed to delete employee.');
    }
}

class EmployeeManager {
    constructor() {
        this.apiUrl = apiBase; // Use apiBase for consistency
    }

    fetchEmployees() {
        fetch(`${this.apiUrl}/employees`)
            .then(response => response.json())
            .then(data => this.renderEmployeeTable(data))
            .catch(error => console.error('Error fetching employees:', error));
    }

    renderEmployeeTable(employees) {
        const tbody = document.querySelector('#employee-table tbody');
        tbody.innerHTML = '';
        employees.forEach(employee => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${employee.id}</td>
                <td>${employee.firstName} ${employee.lastName}</td>
                <td>${employee.email}</td>
                <td>${employee.department}</td>
                <td>${employee.position}</td>
                <td><button onclick="employeeManager.deleteEmployee(${employee.id})">Delete</button></td>
            `;
            tbody.appendChild(row);
        });
    }

    addEmployee(event) {
        event.preventDefault();
        const employee = {
            firstName: document.getElementById('emp-firstname').value,
            lastName: document.getElementById('emp-lastname').value,
            email: document.getElementById('emp-email').value,
            phone: document.getElementById('emp-phone').value, // Added phone field
            department: document.getElementById('emp-department').value,
            position: document.getElementById('emp-position').value,
            salary: parseFloat(document.getElementById('emp-salary').value), // Added salary field
            hireDate: new Date(document.getElementById('emp-hiredate').value).toISOString(), // Added hireDate field
            isActive: document.getElementById('emp-isactive').checked, // Added isActive field
            passwordHash: document.getElementById('emp-password').value, // Added passwordHash field
            role: document.getElementById('emp-role').value // Role is now mandatory
        };

        fetch(`${this.apiUrl}/employees`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwtToken}`
            },
            body: JSON.stringify(employee)
        })
        .then(response => {
            if (response.ok) {
                this.fetchEmployees();
                document.getElementById('add-employee-form').reset();
            } else {
                console.error('Error adding employee:', response.status, response.statusText); // Debugging log for response status
                response.text().then(text => console.error('Response body:', text)); // Debugging log for response body
            }
        })
        .catch(error => console.error('Error adding employee:', error));
    }

    deleteEmployee(id) {
        fetch(`${this.apiUrl}/employees/${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${jwtToken}`
            }
        })
        .then(response => {
            if (response.ok) {
                this.fetchEmployees();
            } else {
                console.error('Error deleting employee:', response.statusText);
            }
        })
        .catch(error => console.error('Error deleting employee:', error));
    }
}

const employeeManager = new EmployeeManager(); // No need to pass apiBase explicitly

document.getElementById('add-employee-form').addEventListener('submit', event => employeeManager.addEmployee(event));

employeeManager.fetchEmployees();
