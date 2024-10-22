
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key] // Указывает, что это первичный ключ в базе данных
    public int Id { get; set; }

    [Required] // Обязательное поле
    public string Username { get; set; }

    [Required] // Обязательное поле
    public int Password { get; set; }
    public User(string username, int password)
    {
        Username = username;
        Password = password;
    }

    public ICollection<UserVideo> UserVideos { get; set; } // Связь многие ко многим
}