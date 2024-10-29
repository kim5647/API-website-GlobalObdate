using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly VideoService _videoService;

    public VideoController(VideoService videoService)
    {
        _videoService = videoService;
    }
    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadAndTrimVideo(IFormFile video, [FromForm] string startTime, [FromForm] string endTime)
    {
        try
        {
            var trimmedFilePath = await _videoService.TrimVideoAsync(video, startTime, endTime);
            var fileResult = await System.IO.File.ReadAllBytesAsync(trimmedFilePath);

            var fileStream = new MemoryStream(fileResult);
            var contentType = "video/mp4";
            var fileName = $"trimmed_{Path.GetFileName(video.FileName)}";

            System.IO.File.Delete(trimmedFilePath);

            return File(fileStream, contentType, fileName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    [Authorize]
    [HttpPost("save")]
    public async Task<IActionResult> SaveVideo(IFormFile video, [FromForm] string username)
    {
        try
        {
            var filePath = await _videoService.SaveVideoAsync(video, $"G:/video/{username}", username);
            return Ok("Video saved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    [Authorize]
    [HttpGet("{filename}")]
    public IActionResult GetVideo(string filename)
    {
        var filePath = Path.Combine("G:/video", filename);

        // Проверяем существование файла
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(); // Возвращаем 404, если файл не найден
        }

        // Используем using для автоматического освобождения ресурсов
        try
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Возвращаем файл как поток
            return File(fileStream, "video/mp4", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            // Обрабатываем исключения, например, доступ к файлу
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    
    [HttpPost("check")]
    public IActionResult Check([FromForm] string Name)
    {
        try
        {
            return Ok(Name);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    [Authorize]
    [HttpPost("check/login")]
    public IActionResult CheckLogin([FromForm] string Name)
    {
        try
        {
            return Ok(Name);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
