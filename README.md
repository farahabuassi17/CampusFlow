# CampusFlow – Student Management System

## Project Overview

CampusFlow is an ASP.NET Core MVC web application developed to manage students, courses, enrollments, and system users.

The project focuses on organizing data management, implementing user authentication and authorization, and keeping the application structure clear and maintainable.

## What I Implemented

### 1. Student and Course Management
- Implemented CRUD operations for students and courses.
- Added student search and filtering.
- Supported uploading and displaying student images.
- Used asynchronous operations for database access.

### 2. Enrollment Management
- Created an Enrollment entity to connect students with courses.
- Implemented CRUD operations for enrollments.
- Prevented duplicate enrollments.
- Restricted enrollment modifications to Admin users.

### 3. Authentication and Authorization
- Integrated ASP.NET Core Identity for registration, login, and logout.
- Implemented Admin and User roles.
- Applied role-based authorization to protect specific actions and pages.
- Controlled access to management features based on user roles.

### 4. User Management
- Implemented user creation, listing, editing, and soft deletion.
- Used ViewModels to handle user data separately from database entities.
- Prevented users marked as deleted from logging in again.

### 5. Application Structure
- Used Entity Framework Core with SQL Server for database operations.
- Applied the Repository Pattern to organize student and course data access.
- Integrated AutoMapper to simplify mapping between Models and ViewModels.
- Used Dependency Injection to manage application services.
- Added an email service with configurable SMTP settings.

## Technologies Used

C# · ASP.NET Core MVC · .NET 10 · Entity Framework Core · SQL Server · ASP.NET Core Identity · AutoMapper · Razor Views · Bootstrap

## What I Learned

Through this project, I practiced building an MVC application with structured data access, managing relationships between entities, implementing role-based security, and improving code organization using repositories, ViewModels, and AutoMapper.
