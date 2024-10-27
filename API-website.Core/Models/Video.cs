namespace API_website.Core.Models;
public class Video
{
    public Video(int id, string nameVideo, string pathVideo, int userId)
    {
        Id = id;
        NameVideo = nameVideo;
        PathVideo = pathVideo;
        UserId = userId;
    }
    public Video(string nameVideo, string pathVideo, int userId)
    {
        NameVideo = nameVideo;
        PathVideo = pathVideo;
        UserId = userId;
    }
    public int Id { get; set; }
    public string NameVideo { get; set; }
    public string PathVideo { get; set; }
    public int UserId { get; set; }
}