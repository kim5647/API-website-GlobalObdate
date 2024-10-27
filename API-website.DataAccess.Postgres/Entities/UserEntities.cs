using System.ComponentModel.DataAnnotations;


namespace API_website.DataAccess.Postgres.Entities;
public class UserEntities
{
    [Key] // Указывает, что это первичный ключ в базе данных
    public int Id { get; set; }

    [Required] // Обязательное поле
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required] // Обязательное поле
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Password { get; set; } = string.Empty;

    public List<VideoEntities> Videos { get; set; } = [];

}