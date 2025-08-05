# Employee Management REST API

## Overview
The Employee Management REST API is a .NET Core application designed to manage employee data. It provides endpoints for CRUD operations and integrates seamlessly with a Bootstrap-based frontend.

## Features
- **Authentication**: Login and registration functionality.
- **Employee Management**: Add, view, update, and delete employees.
- **Frontend Integration**: A responsive HTML page consuming the API.
- **OOP Principles**: Backend and frontend refactored for modularity and maintainability.

## Technologies Used
- **Backend**: .NET Core, Entity Framework Core, MySQL.
- **Frontend**: HTML, CSS (Bootstrap), JavaScript.
- **Authentication**: JWT.

## Setup Instructions
### Prerequisites
- .NET SDK
- MySQL Server
- Node.js (optional for additional tooling)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/soumo210340/Employee-Management-REST-API.git
   ```
2. Navigate to the project directory:
   ```bash
   cd Employee-Management-REST-API
   ```
3. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=EmployeeDB;User=soumyadeep;Password=26558925;"
   }
   ```
4. Apply migrations:
   ```bash
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

## API Endpoints
### Authentication
- **POST** `/api/auth/login`: Login.
- **POST** `/api/auth/register`: Register.

### Employees
- **GET** `/api/employees`: Get all employees.
- **POST** `/api/employees`: Add a new employee.
- **PUT** `/api/employees/{id}`: Update an employee.
- **DELETE** `/api/employees/{id}`: Delete an employee.

## Frontend Usage
1. Open `index.html` in a browser.
2. Use the login or register forms to authenticate.
3. Manage employees using the table and forms provided.

## Contribution
Feel free to fork the repository and submit pull requests for improvements.

## License
This project is licensed under the MIT License.
