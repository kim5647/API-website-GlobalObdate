using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using API_website.Core.Models;
using Xabe.FFmpeg;
public class VideoService
{
    private readonly IVideoRepository _videoRepository; // ������������� ����
    public VideoService(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository; // ������������� � ���������� ������
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

    public async Task<string> SaveVideoAsync(IFormFile video, string destinationPath, string username)
    {
        if (video == null || video.Length <= 0)
        {
            throw new ArgumentException("Invalid video file.");
        }

        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        var filePath = Path.Combine(destinationPath, video.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        int userId = await _videoRepository.GetUserId(username);

        var dbvideo = new Video(video.FileName, filePath, userId);

        await _videoRepository.AddPathVideo(dbvideo);

        return filePath;
    }
}
