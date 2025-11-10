# 🎬 Movizone - Movie & TV Series Streaming Platform

A modern, full-stack streaming platform built with ASP.NET Core and Next.js, featuring a clean architecture design and comprehensive content management system.

## 📋 Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Development](#development)
- [Deployment](#deployment)
- [API Documentation](#api-documentation)
- [Security](#security)
- [Contributing](#contributing)

## ✨ Features

### For Users
- 🎥 Browse extensive movie and TV series catalog
- 🔍 Advanced search and filtering by genre, year, rating
- ⭐ Rate and review content
- 📝 Add items to watchlist/favorites
- 👤 User authentication and profiles
- 📱 Responsive design for all devices
- 🌐 Multi-language support

### For Administrators
- 📊 Comprehensive dashboard with analytics
- ➕ Content management (movies, series, episodes)
- 👥 User management
- 🎭 Actor database management
- 💬 Review and comment moderation
- ⚙️ Site settings configuration
- 📈 Monthly statistics and trends

## 🛠 Tech Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: PostgreSQL (with InMemory fallback for development)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT + Session-based
- **Logging**: Serilog
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Password Hashing**: BCrypt.Net

### Frontend
- **Framework**: Next.js 14 (App Router)
- **UI Library**: React 18
- **Language**: TypeScript 5
- **Styling**: TailwindCSS 3.4
- **Components**: Radix UI
- **Icons**: Lucide React
- **Forms**: React Hook Form + Zod
- **Charts**: Recharts

### DevOps
- **Containerization**: Docker
- **CI/CD**: GitHub Actions
- **Version Control**: Git

## 🏗 Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
MovizoneApp/
├── Core/                    # Domain Layer
│   ├── Entities/           # Business entities
│   ├── Interfaces/         # Repository contracts
│   └── Exceptions/         # Custom exceptions
├── Infrastructure/          # Data Access Layer
│   ├── Repositories/       # Repository implementations
│   └── Services/           # Infrastructure services
├── Application/            # Business Logic Layer
│   ├── Services/           # Application services
│   ├── Interfaces/         # Service contracts
│   └── Mappings/          # AutoMapper profiles
├── Controllers/            # Presentation Layer
├── Views/                  # Razor views
├── DTOs/                   # Data Transfer Objects
└── Middleware/            # Custom middleware

Frontend/
├── app/                    # Next.js pages (App Router)
├── components/             # React components
├── lib/                    # Utilities and API clients
├── hooks/                  # Custom React hooks
├── contexts/               # React contexts
└── types/                  # TypeScript definitions
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+ and npm
- PostgreSQL 13+ (optional, can use InMemory database)
- Docker (for containerized deployment)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/movizone.mvc.git
   cd movizone.mvc
   ```

2. **Backend Setup**
   ```bash
   cd MovizoneApp
   
   # Restore dependencies
   dotnet restore
   
   # Apply database migrations (if using PostgreSQL)
   dotnet ef database update
   
   # Run the application
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   # Return to root directory
   cd ..
   
   # Install dependencies
   npm install
   
   # Run development server
   npm run dev
   ```

4. **Access the application**
   - Backend API: http://localhost:5000
   - Frontend: http://localhost:3000
   - Admin Panel: http://localhost:5000/Admin/Login

## ⚙️ Configuration

### Environment Variables

Create a `.env` file in the root directory (see `.env.example` for reference):

```env
# Database Configuration
USE_POSTGRES=true
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=movizone;Username=user;Password=password

# JWT Configuration
Jwt__SecretKey=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
Jwt__Issuer=MovizoneApp
Jwt__Audience=MovizoneAppUsers
Jwt__ExpirationMinutes=120

# CORS Configuration
CORS__AllowedOrigins=https://yourdomain.com

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

### Database Configuration

**Development (InMemory)**:
- Set `USE_POSTGRES=false` in configuration
- No additional setup required
- Data is reset on application restart

**Production (PostgreSQL)**:
- Set `USE_POSTGRES=true`
- Configure connection string in environment variables
- Run migrations: `dotnet ef database update`

### Frontend Configuration

Edit `next.config.js` to configure:
- Image domains for optimization
- CORS headers
- Security policies

## 💻 Development

### Running Tests

```bash
# Backend tests (when implemented)
cd MovizoneApp
dotnet test

# Frontend tests
npm test
```

### Code Style

```bash
# Backend formatting
dotnet format

# Frontend linting
npm run lint

# Frontend type checking
npm run type-check
```

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName
```

## 🚢 Deployment

### Docker Deployment

```bash
# Build the Docker image
docker build -t movizone:latest .

# Run the container
docker run -d -p 5000:80 \
  -e USE_POSTGRES=true \
  -e ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=movizone;Username=postgres;Password=password" \
  -e Jwt__SecretKey="YourProductionSecretKey" \
  movizone:latest
```

### Docker Compose

```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down
```

### Manual Deployment

1. Build backend:
   ```bash
   cd MovizoneApp
   dotnet publish -c Release -o ./publish
   ```

2. Build frontend:
   ```bash
   npm run build
   ```

3. Deploy the `publish` folder and `.next` folder to your server

## 📚 API Documentation

### Authentication Endpoints

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/logout` - User logout

### Movie Endpoints

- `GET /Movie/Catalog` - Browse movies
- `GET /Movie/Details/{id}` - Get movie details
- `POST /Movie/AddReview` - Add movie review

### Admin Endpoints

- `GET /Admin` - Admin dashboard
- `GET /Admin/Movies` - Manage movies
- `POST /Admin/CreateMovie` - Create new movie
- `PUT /Admin/EditMovie` - Update movie
- `DELETE /Admin/DeleteMovie/{id}` - Delete movie

## 🔒 Security

### Implemented Security Features

- ✅ JWT-based authentication with token validation
- ✅ Password hashing with BCrypt
- ✅ HTTPS enforcement in production
- ✅ CORS policy configuration
- ✅ XSS protection headers
- ✅ CSRF protection (built-in ASP.NET Core)
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ Environment-based configuration (no secrets in code)

### Security Best Practices

1. **Never commit secrets** - Use environment variables
2. **Use strong JWT secrets** - Minimum 32 characters
3. **Configure CORS properly** - Specify allowed origins in production
4. **Enable HTTPS** - Always use HTTPS in production
5. **Regular updates** - Keep dependencies up to date

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

### Code Guidelines

- Follow Clean Architecture principles
- Write meaningful commit messages
- Add tests for new features
- Update documentation as needed
- Follow C# and TypeScript naming conventions

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Your Name** - *Initial work*

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Next.js team for the powerful React framework
- All contributors and supporters

## 📞 Support

For support, email support@movizone.com or open an issue in the GitHub repository.

---

**Note**: This project is optimized for educational and demonstration purposes. For production use, ensure proper security audits and compliance with relevant regulations.
