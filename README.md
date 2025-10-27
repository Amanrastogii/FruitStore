# FruitStore 🍎🍊 – ASP.NET Core E-Commerce Project

FruitStore is a feature-rich e-commerce application built with ASP.NET Core and PostgreSQL. It provides full CRUD management of fruit inventory, robust authentication for admins and customers, well-structured API endpoints, and seamless cloud deployment using Render.

## Features

- **Full CRUD operations** for fruits: Create, Read, Update, and Delete fruit entries
- **DTO-driven API architecture** for clean data transfer & model abstraction
- **Role-based authentication:** Admin and Customer users with secure login
- **PostgreSQL database integration** via Entity Framework Core
- **Image management:** Fruits can store/display image URLs or string data for UI
- **Swagger API documentation** for easy testing and integration
- **CORS enabled** for smooth frontend-backend integration
- **Cloud ready:** Supports deployment to [Render](https://render.com)
- **Environment variable configuration** for secure database connections

## Tech Stack

- **Backend:** ASP.NET Core, C#, Entity Framework Core
- **Database:** PostgreSQL
- **Hosting:** Render
- **Dev Tools:** Visual Studio Code, GitHub, dotnet CLI

## Project Structure

- `/FruitStore.API` : ASP.NET Core Web API and business logic
- `/FruitStore.Core` : Domain models and DTOs
- `/FruitStore.Infrastructure` : Data access, EF Core, Repository Pattern
- `/FruitStore.Tests` : Unit and Integration tests

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- [dotnet-ef CLI](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

### Local Setup

1. **Clone the repo:**
    ```bash
    git clone https://github.com/YOUR_GITHUB_USERNAME/fruitstore.git
    cd fruitstore
    ```

2. **Update connection string**  
   In `appsettings.json` or your Render environment variables, set:
    ```
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=fruitstore_dev;Username=your_pg_user;Password=your_pg_password;"
    }
    ```
    *(Use your Render PostgreSQL config for cloud deployment)*

3. **Apply migrations:**
    ```bash
    dotnet ef database update
    ```

4. **Run the API:**
    ```bash
    dotnet run --project FruitStore.API
    ```

5. **Access Swagger UI:**  
   Visit `http://localhost:5000/swagger` in your browser to explore and test API endpoints.

### Deployment

- Deploy on [Render](https://render.com) using your GitHub repository.
- Set environment variables for connection string and secrets in Render dashboard.
- For database migrations, use Render’s build/run scripts or manual CLI commands.

## API Endpoints (Examples)

- `GET /api/fruits` — List all fruits
- `POST /api/fruits` — Add a new fruit
- `PUT /api/fruits/{id}` — Update fruit entry
- `DELETE /api/fruits/{id}` — Delete fruit entry

## Contributing

Pull requests, feature suggestions, and bug reports are welcome!  
Open an issue or submit a PR.

## License

This project is licensed under the MIT License.

***

You can further customize this README with your GitHub username and any project-specific details. Just select all of the above and paste it into your `README.md`.
