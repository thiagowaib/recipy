# 🧑‍🍳 Recipy API

**Recipy** is a Web API built with **.NET 9** and **C#**, designed to store and manage cooking recipes.  
The goal of this project is to explore modern .NET development practices, focusing on authentication, clean architecture, and database integration with **Microsoft SQL Server**.

---

## 📖 Overview

Recipy allows users to **register**, **authenticate**, and **share recipes** using secure JWT-based authentication.  
All data is persisted in a **SQL Server** database managed through **Entity Framework Core**.

---

## 🚀 Primary Features

### 👤 User Management
- **Register new users** with hashed passwords  
- **Authenticate** and receive a **JWT token**  
- **Refresh tokens** to maintain active sessions  

### 🍲 Recipe Management
- **Public endpoints** to browse recipes  
- **Authenticated endpoints** for adding new recipes  
- Recipes are linked to their creator’s user ID  

### 🔒 Authentication
- JWT tokens for secure access control  
- Role-based endpoint protection  
- Configurable secret key via `.env` file  

### 🧱 Architecture & Patterns
- Clean project organization (`Controllers`, `Dto`, `Repositories`, `Services`)  
- Dependency injection for context and services  
- Asynchronous database operations  
- Configurable environment variables for secrets and database credentials  

---

## 🧩 Project Structure

```

Recipy/
├── Controllers/
│   ├── UsersController.cs
│   └── RecipesController.cs
├── Database/
│   ├── Migrations/
│   └── RecipyContext.cs
├── Dto/
│   ├── UserLoginDto.cs
│   ├── UserRegisterDto.cs
│   └── RecipeDto.cs
├── Models/
│   ├── User.cs
│   └── Recipe.cs
├── Repositories/
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── Services/
│   ├── AuthService.cs
│   └── JwtService.cs
└── Program.cs

````

---

## 🛠️ Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/<your-username>/recipy-api.git
cd recipy-api
````

### 2. Set up the environment variables

Create a `.env` file in the project root with the following values:

```env
# Database credentials
DB_PASSWORD=<Your-8-Char-Mininum-Password>

# JWT Secret
JWT__SECRET=<Your-Jwt-Secret>
```

### 3. Start the SQL Server container

```bash
docker compose up -d
```

### 4. Run the API

```bash
dotnet restore
dotnet build
dotnet run
```

The API should be available at:

```
http://localhost:5000
```

---

## 📘 API Documentation

Recipy uses **Swagger** for automatic API documentation.

Once the API is running locally, open your browser and visit:

👉 [http://localhost:5000/swagger](http://localhost:5000/swagger)

You’ll find all available endpoints, their expected request bodies, and example responses.

---

## 💡 Next Steps

* Add containerization for the .NET API
* Implement a refresh-token mechanism
* Add unit tests for controllers and repositories
* Improve Swagger documentation with summaries and examples

---

## 🧾 License

This project is distributed under the **GPL-3.0** — feel free to use it for learning or your own projects.

---

> Made with ☕ by [Thiago Waib](https://github.com/thiagowaib)