# Database Configuration

This application supports both **InMemory** and **PostgreSQL** databases.

## Development (Default)

By default, the application uses **InMemory database** for easy development without requiring PostgreSQL installation.

```bash
# No configuration needed - just run the app
dotnet run
```

## Production with PostgreSQL

To use PostgreSQL in production:

### Method 1: Environment Variable

Set the environment variable:

```bash
# Linux/Mac
export USE_POSTGRES=true
dotnet run

# Windows (PowerShell)
$env:USE_POSTGRES="true"
dotnet run

# Windows (CMD)
set USE_POSTGRES=true
dotnet run
```

### Method 2: Configuration File

Update `appsettings.json` or `appsettings.Production.json`:

```json
{
  "USE_POSTGRES": true,
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=movizone_db;Username=postgres;Password=your-password"
  }
}
```

### Method 3: Launch Settings (for Visual Studio/Rider)

Update `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "MovizoneApp": {
      "environmentVariables": {
        "USE_POSTGRES": "true"
      }
    }
  }
}
```

## PostgreSQL Setup

If you need to install PostgreSQL:

### Ubuntu/Debian
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo -u postgres psql -c "CREATE DATABASE movizone_db;"
```

### macOS (Homebrew)
```bash
brew install postgresql@15
brew services start postgresql@15
psql postgres -c "CREATE DATABASE movizone_db;"
```

### Windows
Download and install from: https://www.postgresql.org/download/windows/

### Docker
```bash
docker run --name movizone-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=movizone_db -p 5432:5432 -d postgres:15
```

## Connection String Format

```
Host=hostname;Port=5432;Database=database_name;Username=username;Password=password
```

### Example Connection Strings

**Local Development:**
```
Host=localhost;Port=5432;Database=movizone_db;Username=postgres;Password=postgres
```

**Docker:**
```
Host=localhost;Port=5432;Database=movizone_db;Username=postgres;Password=postgres
```

**Azure PostgreSQL:**
```
Host=your-server.postgres.database.azure.com;Port=5432;Database=movizone_db;Username=your-user@your-server;Password=your-password;SslMode=Require
```

**AWS RDS:**
```
Host=your-instance.region.rds.amazonaws.com;Port=5432;Database=movizone_db;Username=your-user;Password=your-password;SslMode=Require
```

## Optimizations Applied

When using PostgreSQL, the following optimizations are active:

1. **Database-level aggregation** - Average ratings calculated in DB
2. **PostgreSQL ILike** - Case-insensitive search with index support
3. **AsNoTracking** - Reduced memory overhead for read operations
4. **Optimized queries** - Distinct genres, similar movies at DB level
5. **Retry logic** - Automatic retry on connection failures (5 attempts, 30s max)

## Troubleshooting

### Connection Failed
- Verify PostgreSQL is running: `sudo systemctl status postgresql` (Linux) or `brew services list` (Mac)
- Check connection string in appsettings.json
- Verify credentials and database exists
- Check firewall settings

### Permission Denied
```bash
# Reset postgres password
sudo -u postgres psql
ALTER USER postgres PASSWORD 'postgres';
```

### Database doesn't exist
```bash
# Create database
psql -U postgres -c "CREATE DATABASE movizone_db;"
```
