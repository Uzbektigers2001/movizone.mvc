using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users;

        public UserService()
        {
            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@hotflix.com",
                    Password = "admin123", // In production, use hashed passwords
                    Role = "Admin",
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Name = "John Doe",
                    Email = "john@example.com",
                    Password = "user123",
                    Role = "User",
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    Name = "Jane Smith",
                    Email = "jane@example.com",
                    Password = "user123",
                    Role = "User",
                    CreatedAt = DateTime.Now.AddMonths(-2),
                    IsActive = true
                },
                new User
                {
                    Id = 4,
                    Name = "Mike Johnson",
                    Email = "mike@example.com",
                    Password = "user123",
                    Role = "User",
                    CreatedAt = DateTime.Now.AddMonths(-1),
                    IsActive = true
                },
                new User
                {
                    Id = 5,
                    Name = "Sarah Williams",
                    Email = "sarah@example.com",
                    Password = "user123",
                    Role = "User",
                    CreatedAt = DateTime.Now.AddDays(-15),
                    IsActive = false
                }
            };
        }

        public List<User> GetAllUsers() => _users;

        public User? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public User? GetUserByEmail(string email) => _users.FirstOrDefault(u => u.Email == email);

        public void AddUser(User user)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            user.CreatedAt = DateTime.Now;
            _users.Add(user);
        }

        public void UpdateUser(User user)
        {
            var existingUser = GetUserById(user.Id);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;
                existingUser.IsActive = user.IsActive;
                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = user.Password;
                }
            }
        }

        public void DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                _users.Remove(user);
            }
        }

        public User? Authenticate(string email, string password)
        {
            return _users.FirstOrDefault(u => u.Email == email && u.Password == password && u.IsActive);
        }
    }
}
