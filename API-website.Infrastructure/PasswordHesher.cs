using BCrypt.Net;
using API_website.Application.Interfaces.Auth;

namespace API_website.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Generate(string password) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }
}
