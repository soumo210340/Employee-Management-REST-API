# Employee Management REST API - Use Cases

## 1. User Registration
- A new user registers with a username, email, password, and role (default: User).
- The system checks for unique username and email.
- On success, the user can log in.

## 2. User Login
- A registered user logs in with username and password.
- On success, the system returns a JWT token for authentication.

## 3. View Employee List (User & Admin)
- Any authenticated user (User or Admin) can view the list of all employees.

## 4. View Employee Details (User & Admin)
- Any authenticated user can view details of a specific employee by ID.

## 5. Add Employee (Admin Only)
- An authenticated Admin can add a new employee by providing required details.
- The system validates the input and creates the employee record.

## 6. Update Employee (Admin Only)
- An authenticated Admin can update an existing employee's details by ID.
- The system validates the input and updates the employee record.

## 7. Delete Employee (Admin Only)
- An authenticated Admin can delete (deactivate) an employee by ID.
- The system marks the employee as inactive.

## 8. Error Handling
- All endpoints return appropriate error messages and status codes for invalid input, unauthorized access, or server errors.

## 9. Frontend Usage
- Users can log in and view/add employees using the provided HTML frontend.

## 10. API Testing
- All endpoints can be tested via Swagger UI or Postman.

---

If you need more detailed user stories or sequence diagrams, let me know!
