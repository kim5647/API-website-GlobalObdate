namespace API_website.Core.Models;
public class User
{
    public User(int id, string username, string password)
    {
        Id = id;
        Username = username;
        Password = password;
    }
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }


}