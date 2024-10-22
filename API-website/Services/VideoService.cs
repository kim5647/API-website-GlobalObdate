using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

public class VideoService
{
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

    public async Task<string> SaveVideoAsync(IFormFile video, string destinationPath)
    {
        if (video == null || video.Length <= 0)
        {
            throw new ArgumentException("Invalid video file.");
        }

        var filePath = Path.Combine(destinationPath, video.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        return filePath;
    }
}
