using Microsoft.EntityFrameworkCore;
using API_website.DataAccess.Postgres.Mapper.UserProfile;
using API_website.Application.Interfaces.Repositories;
using API_website.DataAccess.Postgres.Entities;
using API_website.Core.Models;
using AutoMapper;


namespace API_website.DataAccess.Postgres.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _dbContext;
        private readonly IMapper _mapping;

        public UserRepository(DBContext dbContext, IMapper mappingProfile)
        {
            _dbContext = dbContext;
            _mapping = mappingProfile;
        }

        // Получение всех пользователей с преобразованием в User
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var userEntities = await _dbContext.Users.ToListAsync();
            return userEntities.Select(e => new User(e.Id, e.Username, e.Password));
        }
        public async Task<User?> GetUserAsUsername(string username)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            return user == null ? null : _mapping.Map<User>(user);
        }

        // Создание нового пользователя
        public async Task CreateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var userEntity = _mapping.Map<UserEntities>(user);

                await _dbContext.Users.AddAsync(userEntity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Error saving user to the database.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the user.", ex);
            }
        }

        // Обновление существующего пользователя
        public async Task UpdateUserAsync(User user)
        {
            var userEntity = await _dbContext.Users.FindAsync(user.Id);
            if (userEntity == null)
                throw new KeyNotFoundException($"User with Id {user.Id} not found.");

            userEntity.Username = user.Username;
            userEntity.Password = user.Password;

            _dbContext.Users.Update(userEntity);
            await _dbContext.SaveChangesAsync();
        }

        // Удаление пользователя по ID
        public async Task DeleteUserAsync(int userId)
        {
            var userEntity = await _dbContext.Users.FindAsync(userId);
            if (userEntity != null)
            {
                _dbContext.Users.Remove(userEntity);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"User with Id {userId} not found.");
            }
        }

        // Получение пользователя по ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            var userEntity = await _dbContext.Users.FindAsync(userId);
            return _mapping.Map<User>(userEntity);
        }
        public async Task<int> GetUserId(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) throw new ArgumentException("User not found.");
            return user.Id;
        }
    }
}
