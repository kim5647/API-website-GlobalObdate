using System.ComponentModel.DataAnnotations;

public class User
{
    [Key] // Указывает, что это первичный ключ в базе данных
    public int Id { get; set; }

    [Required] // Обязательное поле
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Username { get; set; }

    [Required] // Обязательное поле
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Password { get; set; }
    public List<Video> Videos { get; set; } = new List<Video>();
}