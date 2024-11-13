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

    // PUT /api/videos/{videoName}/trim - обрезка видео по параметрам времени
    [HttpPut("{videoName}/trim")]
    public async Task<IActionResult> TrimVideo(string videoName, [FromForm] string startTime, [FromForm] string endTime, [FromForm] bool reverse, [FromForm] double slowdownFactor = 1.0)
    {
        try
        {
            // Получаем обрезанное видео с помощью сервиса
            var result = await _videoService.TrimVideoAsync(videoName, startTime, endTime, reverse, slowdownFactor);

            if (string.IsNullOrEmpty(result))
            {
                return NotFound("Video not found or failed to process.");
            }

            // Проверка на существование файла перед открытием
            if (!System.IO.File.Exists(result))
            {
                return StatusCode(500, "Trimmed video file was not created.");
            }

            // Открываем файл и передаем его в ответе
            var fileStream = new FileStream(result, FileMode.Open, FileAccess.Read);
            return File(fileStream, "video/mp4", Path.GetFileName(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Добавляем более подробную информацию об ошибке для отладки
            return StatusCode(500, $"Internal server error: {ex.Message} - {ex.StackTrace}");
        }
    }


    [Authorize]
    [HttpPost("save")]
    public async Task<IActionResult> SaveVideo(IFormFile video)
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
