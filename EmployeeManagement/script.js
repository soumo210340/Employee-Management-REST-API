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
        if (!res.ok) throw new Error('Login failed');
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
        if (!res.ok) throw new Error('Registration failed');
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
    const errorDiv = document.getElementById('employee-error');
    errorDiv.textContent = '';
    try {
        const res = await fetch(`${apiBase}/employees`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwtToken}`
            },
            body: JSON.stringify({ firstName, lastName, email, department, position })
        });
        if (!res.ok) throw new Error('Add failed');
        loadEmployees();
        this.reset();
    } catch (err) {
        errorDiv.textContent = 'Failed to add employee.';
    }
});
async function loadEmployees() {
    const tbody = document.querySelector('#employee-table tbody');
    tbody.innerHTML = '';
    try {
        const res = await fetch(`${apiBase}/employees`, {
            headers: { 'Authorization': `Bearer ${jwtToken}` }
        });
        if (!res.ok) throw new Error('Load failed');
        const employees = await res.json();
        employees.forEach(emp => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${emp.id}</td>
                <td>${emp.firstName} ${emp.lastName}</td>
                <td>${emp.email}</td>
                <td>${emp.department}</td>
                <td>${emp.position}</td>
                <td><button onclick="deleteEmployee(${emp.id})">Delete</button></td>
            `;
            tbody.appendChild(tr);
        });
    } catch (err) {
        tbody.innerHTML = '<tr><td colspan="6">Failed to load employees.</td></tr>';
    }
}
async function deleteEmployee(id) {
    if (!confirm('Delete this employee?')) return;
    try {
        const res = await fetch(`${apiBase}/employees/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${jwtToken}` }
        });
        if (!res.ok) throw new Error('Delete failed');
        loadEmployees();
    } catch (err) {
        alert('Failed to delete employee.');
    }
}
