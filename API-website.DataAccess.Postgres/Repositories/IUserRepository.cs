using System.Collections.Generic;
using System.Threading.Tasks;
using API_website.DataAccess.Postgres.Entities;

//namespace API_website.Application.Interfaces.Repository;
namespace API_website.DataAccess.Postgres.Repositories;
public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
}
