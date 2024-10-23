using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly VideoService _videoService;
    private readonly UserService _userService;

    public VideoController(VideoService videoService, UserService userService)
    {
        _videoService = videoService;
        _userService = userService;
    }

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

    [HttpPost("save")]
    public async Task<IActionResult> SaveVideo(IFormFile video, [FromForm] string username)
    {
        try
        {
            var filePath = await _videoService.SaveVideoAsync(video, $"G:/video/{username}");
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

    [HttpGet("{filename}")]
    public IActionResult GetVideo(string filename)
    {
        var filePath = Path.Combine("G:/video", filename);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "video/mp4");
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromForm] string username, [FromForm] string password)
    {
        try
        {
            await _userService.RegisterUserAsync(username, password);
            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
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
}
