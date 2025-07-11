# Employee Management REST API

A secure, modern Employee Management system built with ASP.NET Core, MySQL, and a clean HTML/CSS/JS frontend.

---

## Features

- **User Authentication:**  
  - JWT-based login and registration.
  - Role-based access (Admin/User).
  - Secure password hashing with BCrypt.
  - Registration and login endpoints for both users and employees.

- **Employee Management:**  
  - Admins can add, edit, and delete employees.
  - All authenticated users can view employee lists.
  - Employee registration creates both a User and Employee record.
  - Employee details: first name, last name, email, phone, department, position, salary, hire date.

- **Frontend:**  
  - Responsive, modern UI using HTML, CSS, and vanilla JS.
  - Login and registration forms with navigation.
  - Employee list and add employee form.
  - Distinct button styles and user-friendly design.

- **Database:**  
  - MySQL backend.
  - Entity Framework Core for ORM.
  - Database seeding and migration support.

- **Security:**  
  - Endpoints protected with `[Authorize]` and role checks.
  - Error handling and logging in controllers.
  - XML documentation for API endpoints.

- **API:**
  - RESTful endpoints for authentication and employee management.
  - Admin-only restrictions for sensitive operations.
  - Swagger/OpenAPI documentation available.

- **Deployment:**
  - Easily configurable for public or local network access.
  - Docker support (see Docker folder).

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- (Optional) [MySQL Workbench](https://dev.mysql.com/downloads/workbench/)

### Setup

1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd Employee-Management-REST-API
   ```

2. **Configure MySQL:**
   - Ensure MySQL is running.
   - Create the database and user (if not already done):
     ```sql
     CREATE DATABASE IF NOT EXISTS EmployeeDb;
     CREATE USER 'soumyadeep'@'localhost' IDENTIFIED BY '26558925';
     GRANT ALL PRIVILEGES ON EmployeeDb.* TO 'soumyadeep'@'localhost';
     FLUSH PRIVILEGES;
     ```

3. **Update Connection String:**
   - In `EmployeeManagement/appsettings.json` and `appsettings.Development.json`:
     ```json
     "DefaultConnection": "Server=localhost;Database=EmployeeDb;User=soumyadeep;Password=26558925;"
     ```

4. **Restore and Build:**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Apply Migrations (if needed):**
   ```bash
   dotnet ef database update --project EmployeeManagement/EmployeeManagement.csproj
   ```

6. **Run the Project:**
   ```bash
   dotnet run --project EmployeeManagement/EmployeeManagement.csproj
   ```

7. **Access the App:**
   - Frontend: [http://localhost:5000/index.html](http://localhost:5000/index.html)
   - Swagger API: [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## Usage

### Authentication

- **Register:**  
  - Use the "Register" button on the login page to create a new user or employee account.
- **Login:**  
  - Enter your username and password to log in and access employee features.

### Employee Management

- **Admins:**  
  - Can add, edit, and delete employees.
- **Users:**  
  - Can view the employee list.

### API Endpoints

- `POST /api/auth/register` — Register a new user
- `POST /api/auth/login` — Login and receive JWT
- `GET /api/employees` — Get all employees (auth required)
- `POST /api/employees/register` — Register a new employee (admin only)
- `POST /api/employees` — Add employee (admin only)
- `PUT /api/employees/{id}` — Update employee (admin only)
- `DELETE /api/employees/{id}` — Delete employee (admin only)

---

## Frontend Tech

- **HTML5**
- **CSS3** (dedicated `style.css` for modern, responsive design)
- **Vanilla JavaScript**

---

## Customization

- **To make the API public:**  
  - Change `applicationUrl` in `launchSettings.json` to `0.0.0.0`.
  - Open firewall ports 5000/5001.
  - Use your machine's IP address for access on your network.

---

## License

MIT License

---

## Credits

- Built with [ASP.NET Core](https://dotnet.microsoft.com/), [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql), and [MySQL](https://www.mysql.com/).

---

## Test Login

You can use the following credentials for testing (if seeded or registered):

- **Username:** admin
- **Password:** admin123

If these do not work, register a new user using the registration form on the login page.

---

> **Note:** User authentication is currently not stored in the `employees` table. If you want to store authentication (username, password, role) in the `employees` table, you will need to add columns such as `Username`, `PasswordHash`, and `Role` to the table and update your backend logic accordingly.
