using API_website.Core.Models;

namespace API_website.Application.Interfaces.Auth;
public interface IJwtProvider
{
    public string GnerateToken(User user);
}
