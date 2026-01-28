# 🍳 Love2Cook - Recipe Management Application

Full-stack web application for managing recipes with smart features.

## 🚀 Features

- 📖 Browse and search recipes with filters
- 🧠 Smart Leftovers - find recipes based on available ingredients
- ⭐ Rate and favorite recipes
- ⏱️ Cooking timer with notifications
- 🔢 Portion calculator with automatic ingredient adjustment
- 🔥 Calorie and nutrition calculator (real data from database)
- 🔐 User authentication and authorization
- 🔧 Admin panel with statistics

## 🛠️ Tech Stack

**Backend:**
- ASP.NET Core 8.0
- SQL Server
- Entity Framework Core
- BCrypt for password hashing

**Frontend:**
- React 18
- Vite
- JavaScript (ES6+)

## 📦 Installation

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- SQL Server 2019+

### Backend Setup

1. Clone the repository:
```bash
   git clone https://github.com/YOUR_USERNAME/Love2Cook-Project.git
   cd Love2Cook-Project
```

2. Create `appsettings.Development.json` in `Love2Cook.API/`:
```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=love2cook_db;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
     }
   }
```

3. Run database migrations:
```bash
   cd Love2Cook.API
   dotnet ef database update
```

4. Start the backend:
```bash
   dotnet run
```
   Or press F5 in Visual Studio

   Backend will run on: `https://localhost:7107`

### Frontend Setup

1. Navigate to frontend directory:
```bash
   cd frontend/love2cook-frontend
```

2. Install dependencies:
```bash
   npm install
```

3. Start dev server:
```bash
   npm run dev
```

   Frontend will run on: `http://localhost:5173`

## 🗄️ Database

The application uses SQL Server with the following main tables:
- `users` - User accounts and authentication
- `recipes` - Recipe information
- `ingredients` - Ingredient database with nutrition data
- `recipe_ingredients` - Recipe-ingredient relationships
- `reviews` - Recipe ratings
- `shopping_lists` - User shopping lists

## 👤 Default Users

After running migrations, you can create users via registration or use SQL:

**Admin user:**
```sql
-- Register via the app, then update role:
UPDATE users SET role_id = 3 WHERE email = 'your@email.com';
```

## 📝 API Documentation

Backend API runs on `https://localhost:7107`

Swagger documentation available at: `https://localhost:7107/swagger`

## 🔒 Security Notes

- **Never commit** `appsettings.Development.json` (contains real connection strings)
- Passwords are hashed using BCrypt
- CORS is configured for development (`localhost:3000` and `localhost:5173`)

## 📱 Features Overview

### User Features
- Recipe browsing with search and filters
- Smart ingredient-based recipe matching
- Favorite recipes management
- Recipe rating system
- Cooking timer with browser notifications
- Dynamic portion calculator
- Real-time calorie and nutrition calculation

### Admin Features
- User management
- Recipe management
- System statistics dashboard

## 📄 License

MIT License

## 👨‍💻 Author

Stetsiuk Taras - https://www.linkedin.com/in/taras-stetsiuk-279504343/

## 🙏 Acknowledgments

- ASP.NET Core Documentation
- React Documentation
- Material Design Guidelines
- Backend: ASP.NET Core 8.0 with SQL Server
- Frontend: React 18 + Vite
- Features: recipes, smart leftovers, admin panel, favorites, timer, ratings, nutrition calculator
- Security: Connection strings hidden, .gitignore configured
