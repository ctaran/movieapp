# MovieApp

A web app for browsing and discovering movies.

## Features

- Browse popular movies
- Search for movies
- View movie details
- Responsive design

## Tech Stack

- React
- TypeScript
- Tailwind CSS
- .NET (C#) Backend
- SQL Server (Docker)
- TMDB API

## Getting Started

Follow these steps to set up and run the project locally:

### 1. Clone the Repository

```bash
git clone https://github.com/ctaran/movieapp.git
cd movieapp
```

### 2. Start SQL Server with Docker

Ensure you have Docker installed. Start the SQL Server container:

```bash
docker-compose up -d
```

This will start a SQL Server instance as defined in `docker-compose.yml`.

### 3. Run the Backend (.NET API)

1. Navigate to the backend project directory:
   ```bash
   cd MovieApp.API
   ```
2. Restore dependencies and run migrations (if needed):
   ```bash
   dotnet restore
   dotnet ef database update
   ```
3. Start the API:
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:5001` (or as configured).

### 4. Run the Frontend (React App)

1. Open a new terminal and navigate to the frontend directory:
   ```bash
   cd movieapp-client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm start
   ```
   The app will be available at `http://localhost:3000`.

---

If you have any questions or issues, please contact Cristian Taran (ctaran). 