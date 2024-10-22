//using System.Threading.Tasks;

public class UserService
{
    private readonly IUserRepository _iUserRepository;

    public UserService(IUserRepository IUserRepository)
    {
        _iUserRepository = IUserRepository;
    }

    public async Task RegisterUserAsync(int id, string username, string password)
    {

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        var user = new User
        {
            Id = id,
            Username = username,
            Password = password
        };

        await _iUserRepository.CreateUserAsync(user);
    }
}
