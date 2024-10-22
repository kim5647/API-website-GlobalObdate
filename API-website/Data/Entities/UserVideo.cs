using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

public class UserVideo
{
    [Required]
    public int UserId { get; set; } // Внешний ключ к User
    public User User { get; set; }

    [Required]
    public int VideoId { get; set; } // Внешний ключ к Video
    public Video Video { get; set; }
}