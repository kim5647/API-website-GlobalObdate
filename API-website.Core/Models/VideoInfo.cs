namespace API_website.Core.Models;

public class VideoInfo
{
    public string NameVideo { get; set; }
    public string Duration { get; set; }
    public string Smalic { get; set; }
    public VideoInfo(string nameVideo, string duration, string smalic)
    {
        NameVideo = nameVideo;
        Duration = duration;
        Smalic = smalic;
    }
}
