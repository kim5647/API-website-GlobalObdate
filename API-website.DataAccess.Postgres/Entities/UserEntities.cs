using System.ComponentModel.DataAnnotations;


namespace API_website.DataAccess.Postgres.Entities;
public class UserEntities
{
    [Key] // ���������, ��� ��� ��������� ���� � ���� ������
    public int Id { get; set; }

    [Required] // ������������ ����
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required] // ������������ ����
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Password { get; set; } = string.Empty;

    public List<VideoEntities> Videos { get; set; } = [];

}