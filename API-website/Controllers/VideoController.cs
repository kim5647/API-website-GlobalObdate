using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;


[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly VideoService _videoService;

    public VideoController(VideoService videoService)
    {
        _videoService = videoService;
    }


    private const string FolderPath = @"G:\Downloads\EZDrummer";
    private const string ZipFilePath = @"G:\Downloads\EZDrummer.zip";

    [HttpGet("EZDrummer")]
    public IActionResult DownloadFolder()
    {
        try
        {
            // Проверка, существует ли ZIP-архив, и создание, если его нет
            if (!System.IO.File.Exists(ZipFilePath))
            {
                ZipFile.CreateFromDirectory(FolderPath, ZipFilePath);
            }

            // Потоковая передача файла напрямую из файловой системы
            var fileStream = new FileStream(ZipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = "application/zip";
            var fileName = "EZDrummer.zip";

            return File(fileStream, contentType, fileName, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
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
            var useridClaim = User.FindFirst("userid");

            if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
            {
                return StatusCode(402, "Not jwt token");
            }

            // Пробуем разобрать значение userid как целое число
            if (!int.TryParse(useridClaim.Value, out int userId))
            {
                return StatusCode(400, "Invalid user ID format in token");
            }

            var filePath = await _videoService.SaveVideoAsync(video, int.Parse(useridClaim.Value));
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

    //[Authorize]
    //[HttpGet("GetAllVideo")]
    //public async Task<IActionResult> GetAllVideo()
    //{
    //    try
    //    {
    //        var videos = await _videoService.GetAllVideosAsync();
    //        return Ok(videos);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, "Internal server error: " + ex.Message);
    //    }
    //}
    [Authorize]
    [HttpGet("GetVideo")]
    public async Task<IActionResult> GetVideo([FromForm] string nameVideo)
    {
        try
        {
            //string nameVideo = "ssstik.io_1730496106114.mp4";

            var useridClaim = User.FindFirst("userid");

            if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
            {
                return StatusCode(402, "Not jwt token");
            }

            // Пробуем разобрать значение userid как целое число
            if (!int.TryParse(useridClaim.Value, out int userId))
            {
                return StatusCode(400, "Invalid user ID format in token");
            }

            var video = await _videoService.GetVideoFileAsync(nameVideo, int.Parse(useridClaim.Value));

            if (video == null)
                return NotFound("Video not found");

            return new FileStreamResult(video, "video/mp4")
            {
                FileDownloadName = nameVideo
            };
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
