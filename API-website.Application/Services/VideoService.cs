using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using API_website.Core.Models;
using Xabe.FFmpeg;
using System.IO;
public class VideoService
{
    private readonly IVideoRepository _videoRepository; // переименовали поле
    private readonly IUserRepository _userRepository;
    public VideoService(IVideoRepository videoRepository, IUserRepository userRepository)
    {
        _videoRepository = videoRepository; // инициализация с корректным именем
        _userRepository = userRepository;
    }
    public async Task<string> TrimVideoAsync(IFormFile video, string startTime, string endTime)
    {
        if (video == null || video.Length <= 0)
        {
            throw new ArgumentException("Invalid video file.");
        }

        var tempFilePath = Path.GetTempFileName();

        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        var trimmedFilePath = Path.GetTempFileName();

        await FFmpeg.Conversions.New()
            .AddParameter($"-i \"{tempFilePath}\"")
            .AddParameter($"-ss \"{startTime}\"")
            .AddParameter($"-to \"{endTime}\"")
            .AddParameter($"-c copy \"{trimmedFilePath}\"")
            .Start();

        File.Delete(tempFilePath);

        return trimmedFilePath;
    }
    public async Task<FileStream> GetVideoFileAsync(string nameVideo, int id)
    {
        var videos = await _videoRepository.GetVideoByNameAsync(nameVideo, id); // метод должен возвращать один объект Video

        if (videos == null || string.IsNullOrEmpty(videos.PathVideo))
        {
            throw new FileNotFoundException("Video file not found.");
        }

        return new FileStream(videos.PathVideo, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    //public async Task GetAllVideosAsync()
    //{
    //    var video = await _videoRepository.GetAllAsync();
    //    return video;
    //}
    private const string PathVideo = "G:/video/";
    public async Task<string> SaveVideoAsync(IFormFile video, int userid)
    {
        var user = await _userRepository.GetUserByIdAsync(userid);

        if (video == null || video.Length <= 0)
        {
            throw new ArgumentException("Invalid video file.");
        }

        string pathVideo = PathVideo + user.Username;

        if (!Directory.Exists(pathVideo))
        {
            Directory.CreateDirectory(pathVideo);
        }

        var filePath = Path.Combine(pathVideo, video.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        var dbvideo = new Video(video.FileName, filePath, userid);

        await _videoRepository.AddPathVideo(dbvideo);

        return filePath;
    }

}
