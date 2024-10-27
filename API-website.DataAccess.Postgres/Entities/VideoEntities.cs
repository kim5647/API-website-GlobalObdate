using API_website.DataAccess.Postgres.Entities;
using System.ComponentModel.DataAnnotations;


namespace API_website.DataAccess.Postgres.Entities;
public class VideoEntities
{
    [Key]
    public int Id { get; set; } // Первичный ключ
    [Required]
    public string NameVideo { get; set; } = string.Empty;
    [Required]
    public string PathVideo { get; set; } = string.Empty;
    [Required]
    public int UserId { get; set; }

    public UserEntities? User { get; set; }
}