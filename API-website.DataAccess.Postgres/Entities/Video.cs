using API_website.DataAccess.Postgres.Entities;
using System.ComponentModel.DataAnnotations;


namespace API_website.DataAccess.Postgres.Entities;
public class Video
{
    [Key]
    public int Id { get; set; } // Первичный ключ
    [Required]
    public string NameVideo { get; set; }
    [Required]
    public string PathVideo { get; set; }
    [Required]
    public int UserId { get; set; }

    public User? User { get; set; }
}