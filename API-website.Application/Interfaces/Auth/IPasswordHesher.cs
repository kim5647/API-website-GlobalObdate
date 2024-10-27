namespace API_website.Application.Interfaces.Auth;


public interface IPasswordHasher
{
    string Generate(string password);
}


