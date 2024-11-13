using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using API_website.Core.Models;
using Xabe.FFmpeg;
using System.IO;
public class VideoService
{
    private readonly IVideoRepository _videoRepository; // ������������� ����
    private readonly IUserRepository _userRepository;
    public VideoService(IVideoRepository videoRepository, IUserRepository userRepository)
    {
        _videoRepository = videoRepository; // ������������� � ���������� ������
        _userRepository = userRepository;
    }
    public async Task<string> TrimVideoAsync(string video, string startTime, string endTime) =>
        await TrimVideoAsync(video, startTime, endTime, reverse: false, slowdownFactor: 1.0);

    // ������������� ������ ��� ������� � ������������ ����������
    public async Task<string> TrimVideoAsync(string video, string startTime, string endTime, double slowdownFactor) => 
        await TrimVideoAsync(video, startTime, endTime, reverse: false, slowdownFactor: slowdownFactor);

    // ������������� ������ ��� ������� � ���������
    public async Task<string> TrimVideoAsync(string video, string startTime, string endTime, bool reverse) =>
        await TrimVideoAsync(video, startTime, endTime, reverse: reverse, slowdownFactor: 1.0);

    // �������� ����� � ������ �����������������
    public async Task<string> TrimVideoAsync(string video, string startTime, string endTime, bool reverse, double slowdownFactor)
    {
        // �������� ���� � �����
        string pathVideo = await _videoRepository.GetPathVideo(video);

        if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
        {
            throw new ArgumentException("Start time and end time must be provided.");
        }

        // ������� ��������� ���� ��� ����������� �����
        var trimmedFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

        try
        {
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{pathVideo}\"")        // ������� ����
                .AddParameter($"-ss {startTime}")           // ����� ������
                .AddParameter($"-to {endTime}");            // ����� �����

            // ��������� ������ �������� ��� ����� � �����, ���� ������� reverse
            if (reverse)
            {
                conversion.AddParameter("-vf reverse")      // ������ �����
                          .AddParameter("-af areverse");    // ������ �����
            }

            if (slowdownFactor != 1.0)
            {
                // �������� �� ��������
                //conversion.AddParameter($"-filter:v setpts={1.0 / 2}*PTS")
                //          .AddParameter($"-filter:a atempo={2}");

            }
            else
            {
                conversion.AddParameter("-c copy");
            }

            // ��������� ���� � ��������� ����� � ��������� ����������
            conversion.SetOutput(trimmedFilePath)
                .SetOverwriteOutput(true);

            // ��������� �����������
            await conversion.Start();

            return trimmedFilePath;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to trim the video.", ex);
        }
    }




    public async Task<FileStream> GetVideoFileAsync(string nameVideo, int id)
    {
        var videos = await _videoRepository.GetVideoByNameAsync(nameVideo, id); // ����� ������ ���������� ���� ������ Video

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
