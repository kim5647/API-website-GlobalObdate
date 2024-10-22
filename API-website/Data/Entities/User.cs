
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key] // ���������, ��� ��� ��������� ���� � ���� ������
    public int Id { get; set; }

    [Required] // ������������ ����
    public string Username { get; set; }

    [Required] // ������������ ����
    public int Password { get; set; }
    public User(string username, int password)
    {
        Username = username;
        Password = password;
    }

    public ICollection<UserVideo> UserVideos { get; set; } // ����� ������ �� ������
}