using System.ComponentModel.DataAnnotations;

public class Video
{
    [Key]
    public Guid Id { get; set; } // Первичный ключ
    [Required]
    public string NameVideo { get; set; }
    [Required]
    public string PathVideo { get; set; }
    public List<User> Users { get; set; } = new List<User>();
}