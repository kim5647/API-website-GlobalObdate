using API_website.Application.Interfaces.Auth;
using API_website.Application.Interfaces.Repositories;
using API_website.Core.Models;
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IUserRepository IUserRepository, IPasswordHasher IPasswordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = IUserRepository;
        _passwordHasher = IPasswordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task RegisterUserAsync(string username, string password)
    {
        var existingUser = await _userRepository.GetUserAsUsername(username);

        if (existingUser != null)
            throw new Exception($"User with username '{username}' already exists.");

        var hashedPassword = _passwordHasher.Generate(password);

        var user = new User(username, hashedPassword);

        await _userRepository.CreateUserAsync(user);
    }
    public async Task<string> Login(string username, string password)
    {
        var user = await _userRepository.GetUserAsUsername(username);

        if (user == null)
            throw new Exception($"Not User");

        var result = _passwordHasher.Verify(password, user.Password);

        if (result == false) throw new Exception("Not users");

        var token = _jwtProvider.GnerateToken(user);

        return (token);
    }
    public async Task<User> GetUserId(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }
}
