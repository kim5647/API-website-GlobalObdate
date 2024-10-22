using System.ComponentModel.DataAnnotations;

public class Video
{
    [Key]
    public int Id { get; set; } // ��������� ����
    [Required]
    public string NameVideo { get; set; }
    [Required]
    public string PathVideo { get; set; }
    [Required]
    public ICollection<UserVideo> UserVideos { get; set; } // ����� ������ �� ������
}