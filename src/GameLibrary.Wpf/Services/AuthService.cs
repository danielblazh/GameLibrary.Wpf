using System.Security.Cryptography;
using System.Text;
using GameLibrary.Wpf.Models;

namespace GameLibrary.Wpf.Services
{
    public class AuthService
    {
        private readonly DatabaseService _db;

        public AuthService(DatabaseService db)
        {
            _db = db;
        }

        public User? CurrentUser { get; private set; }

        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public User? Login(string username, string password)
        {
            var hash = HashPassword(password);
            CurrentUser = _db.GetUser(username, hash);
            return CurrentUser;
        }

        public bool Register(string username, string email, string password, string role = "User")
        {
            if (_db.UsernameExists(username)) return false;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role
            };
            _db.AddUser(user);
            return true;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
