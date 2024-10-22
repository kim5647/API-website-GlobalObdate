using System.Threading.Tasks;

public class UserService
{
    private readonly IUserRepository _iUserRepository;

    public UserService(IUserRepository IUserRepository)
    {
        _iUserRepository = IUserRepository;
    }

    public async Task RegisterUserAsync(string username, int password)
    {
        var user = new User(username, password);
        await _iUserRepository.CreateUserAsync(user);
    }
}
