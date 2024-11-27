using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using API_website.Core.Models;
using Xabe.FFmpeg;
using System.IO;
using System.Globalization;
public class VideoService
{
    private readonly IVideoRepository _videoRepository; // ������������� ����
    private readonly IUserRepository _userRepository;
    public VideoService(IVideoRepository videoRepository, IUserRepository userRepository)
    {
        _videoRepository = videoRepository; // ������������� � ���������� ������
        _userRepository = userRepository;
    }
    public async Task<string> TrimVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // �������� ���� � �����
        string pathVideo = _videoRepository.GetPathVideo(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        var startTime = optionsFfmpeg.StartTime;
        var endTime = optionsFfmpeg.EndTime;

        if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
        {
            throw new ArgumentException("Start time and end time must be provided.");
        }

        // ���������, ��� ���� ����������
        if (!File.Exists(pathVideo))
        {
            throw new FileNotFoundException($"The input video file '{pathVideo}' does not exist.");
        }

        // ��������� ���� ��� ���������� �����
        var directory = Path.GetDirectoryName(pathVideo);
        if (directory == null) throw new FileNotFoundException($"Not directory");
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        try
        {
            // ����������� FFmpeg
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{pathVideo}\"")  // ������� ����
                .AddParameter($"-ss {startTime}")    // ����� ������
                .AddParameter($"-to {endTime}")      // ����� ���������
                .AddParameter("-c copy")            // ����������� ��� ���������������
                .SetOutput(tempFilePath)            // ��������� ���� ��� ���������
                .SetOverwriteOutput(true);          // ��������� ����������

            // ���������� ��������� �����������
            await conversion.Start();

            // ������� ������������ ����
            if (File.Exists(pathVideo))
            {
                File.Delete(pathVideo);
            }

            // ��������������� ��������� ���� � ������������ ���
            File.Move(tempFilePath, pathVideo);

            return pathVideo; // ���������� ���� � ����������� �����
        }
        catch (Exception ex)
        {
            // ������� ��������� ���� � ������ ������
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            throw new InvalidOperationException("Failed to trim the video.", ex);
        }
    }

    // �������� ����� � ������ �����������������

    public async Task<string> ReverseVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // �������� ���� � �����
        string pathVideo = _videoRepository.GetPathVideo(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        if (string.IsNullOrEmpty(pathVideo) || !File.Exists(pathVideo)) 
            throw new FileNotFoundException($"The input video file '{pathVideo}' does not exist.");

        // ��������� ���� ��� ���������� �����
        var directory = Path.GetDirectoryName(pathVideo);
        if (directory == null) throw new FileNotFoundException($"Not directory");
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        try
        {
            // ����������� FFmpeg ��� ������� ����� � �����
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{pathVideo}\"")    // ������� ����
                .AddParameter("-vf reverse")           // ������ �����
                .AddParameter("-af areverse")          // ������ �����
                .SetOutput(tempFilePath)               // ��������� �������� ����
                .SetOverwriteOutput(true);             // ��������� ����������

            // ���������� ��������� �����������
            await conversion.Start();

            // ������� ������������ ����
            if (File.Exists(pathVideo))
            {
                File.Delete(pathVideo);
            }

            // ��������������� ��������� ���� � ������������ ���
            File.Move(tempFilePath, pathVideo);

            return pathVideo; // ���������� ���� � ����������� �����
        }
        catch (Exception ex)
        {
            // ������� ��������� ���� � ������ ������
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            throw new InvalidOperationException("Failed to reverse the video.", ex);
        }
    }



    public async Task<string> SpeedVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // �������� ���� � �����
        string pathVideo = await _videoRepository.GetPathVideoAsync(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        var directory = Path.GetDirectoryName(pathVideo)
                        ?? throw new DirectoryNotFoundException("Could not determine the directory of the video.");
        // ������� ��������� ���� ��� ����������� �����
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        var setptsValue = (1 / optionsFfmpeg.SlowdownFactor).ToString(CultureInfo.InvariantCulture);
        var atempoValue = optionsFfmpeg.SlowdownFactor.ToString(CultureInfo.InvariantCulture);

        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{pathVideo}\"") // ������� ����
            .AddParameter($"-vf \"setpts={setptsValue}*PTS\"") // ����������� (����������)
            .AddParameter($"-af \"atempo={atempoValue}\"")     // ����������� (����������)
            .SetOutput(tempFilePath)                          // ��������� ����
            .SetOverwriteOutput(true);                        // ��������� ����������

        // ��������� �����������
        await conversion.Start();

        // ������� ������������ ����
        if (File.Exists(pathVideo))
        {
            File.Delete(pathVideo);
        }

        // ��������������� ��������� ���� � ������������ ���
        File.Move(tempFilePath, pathVideo);

        return pathVideo;
    }
    public async Task<string> SlowingVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // �������� ���� � �����
        string pathVideo = await _videoRepository.GetPathVideoAsync(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        var directory = Path.GetDirectoryName(pathVideo)
                        ?? throw new DirectoryNotFoundException("Could not determine the directory of the video.");
        // ������� ��������� ���� ��� ����������� �����
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        var slowdownFactor = optionsFfmpeg.SlowdownFactor;

        // �������� ��� ������� setpts
        var setptsValue = (1 / slowdownFactor).ToString(CultureInfo.InvariantCulture);

        // ��������� ������� atempo
        var atempoFilter = GenerateAtempoFilter(slowdownFactor);

        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{pathVideo}\"") // ������� ����
            .AddParameter($"-vf \"setpts={setptsValue}*PTS\"") // ����������� (����������)
            .AddParameter($"-af \"{atempoFilter}\"")          // ����������� (����������)
            .SetOutput(tempFilePath)                          // ��������� ����
            .SetOverwriteOutput(true);                        // ��������� ����������                     // ��������� ����������

        // ��������� �����������
        await conversion.Start();

        // ������� ������������ ����
        if (File.Exists(pathVideo))
        {
            File.Delete(pathVideo);
        }

        // ��������������� ��������� ���� � ������������ ���
        File.Move(tempFilePath, pathVideo);

        return pathVideo;
    }
    private string GenerateAtempoFilter(double slowdownFactor)
    {
        var filters = new List<string>();

        // �������� ��� �����, ���� �������� �� �������� � ��������� �� 0.5 �� 2.0
        while (slowdownFactor < 0.5 || slowdownFactor > 2.0)
        {
            if (slowdownFactor < 0.5)
            {
                filters.Add("atempo=0.5");
                slowdownFactor *= 2; // ����������� � 2 ����
            }
            else if (slowdownFactor > 2.0)
            {
                filters.Add("atempo=2.0");
                slowdownFactor /= 2; // ��������� � 2 ����
            }
        }

        // ��������� ��������� ������ � ���������� ���������
        filters.Add($"atempo={slowdownFactor.ToString(CultureInfo.InvariantCulture)}");

        // ���������� �������
        return string.Join(",", filters);
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
