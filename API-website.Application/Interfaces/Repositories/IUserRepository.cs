using API_website.Core.Models;
namespace API_website.Application.Interfaces.Repositories;
public interface IUserRepository
{
    Task<User> GetUserAsUsername(string username);
    Task<User> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
}
 