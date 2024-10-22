using System.ComponentModel.DataAnnotations;

public class User
{
    [Key] // ���������, ��� ��� ��������� ���� � ���� ������
    public int Id { get; set; }

    [Required] // ������������ ����
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Username { get; set; }

    [Required] // ������������ ����
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Password { get; set; }
    public List<Video> Videos { get; set; } = new List<Video>();
}