# 🍎 FruitStore: ASP.NET Core E-Commerce API

FruitStore is an e-commerce backend built with **ASP.NET Core** and **PostgreSQL**. It provides CRUD management of fruit inventory, role-based authentication for admins and customers, Swagger-documented endpoints, and cloud deployment on Render.

## ✨ Features

- **Full CRUD** for fruits: create, read, update and delete entries
- **DTO-driven API** for clean data transfer and model abstraction
- **Role-based authentication:** Admin and Customer users with secure login
- **Repository pattern** over **Entity Framework Core** with migrations
- **PostgreSQL** database integration
- **Image support:** fruits store image URLs (see `IMAGE_UPLOAD_GUIDE.md`)
- **Swagger** API documentation for easy testing
- **CORS** enabled for frontend integration
- **Docker** support and **Render** deployment
- **Environment-variable configuration** for database connections and secrets

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core, C# |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Hosting | Render, Docker |
| Tooling | dotnet CLI, Swagger |

## 📂 Project Structure

```
FruitStore/
├── Controllers/    # API endpoints
├── DTOs/           # Request and response models
├── Models/         # Domain entities
├── Data/           # EF Core DbContext
├── Repositories/   # Data-access layer (repository pattern)
├── Migrations/     # EF Core migrations
└── Dockerfile
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- [dotnet-ef CLI](https://learn.microsoft.com/ef/core/cli/dotnet)

### Local setup

1. **Clone the repo**

```bash
git clone https://github.com/Amanrastogii/FruitStore.git
cd FruitStore
```

2. **Set the connection string** in `appsettings.json` (or as an environment variable):

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=fruitstore_dev;Username=your_pg_user;Password=your_pg_password;"
}
```

For Render, use your Render PostgreSQL connection details as environment variables.

3. **Apply migrations**

```bash
dotnet ef database update
```

4. **Run the API**

```bash
dotnet run
```

5. **Open Swagger UI** at the `/swagger` path of the URL printed in the console.

## ☁️ Deployment

- Deploy on [Render](https://render.com) from this GitHub repository (a `Dockerfile` is included).
- Set the connection string and secrets as environment variables in the Render dashboard.
- Apply database migrations with `dotnet ef database update` or a build/run script.

## 📡 API Endpoints (examples)

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/fruits` | List all fruits |
| `POST` | `/api/fruits` | Add a new fruit |
| `PUT` | `/api/fruits/{id}` | Update a fruit |
| `DELETE` | `/api/fruits/{id}` | Delete a fruit |

## 👨‍💻 Author

**Aman Rastogi**: [GitHub](https://github.com/Amanrastogii) · [LinkedIn](https://www.linkedin.com/in/amanrastogi-dev)

## 📄 License

Released under the MIT License.
