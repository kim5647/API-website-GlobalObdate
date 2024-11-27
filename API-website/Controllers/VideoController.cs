using API_website.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

[Authorize]
[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly VideoService _videoService;

    public VideoController(VideoService videoService)
    {
        _videoService = videoService;
    }


    //private const string FolderPath = @"G:\Downloads\EZDrummer";
    //private const string ZipFilePath = @"G:\Downloads\EZDrummer.zip";

    //[HttpGet("EZDrummer")]
    //public IActionResult DownloadFolder()
    //{
    //    try
    //    {
    //        // Проверка, существует ли ZIP-архив, и создание, если его нет
    //        if (!System.IO.File.Exists(ZipFilePath))
    //        {
    //            ZipFile.CreateFromDirectory(FolderPath, ZipFilePath);
    //        }

    //        // Потоковая передача файла напрямую из файловой системы
    //        var fileStream = new FileStream(ZipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    //        var contentType = "application/zip";
    //        var fileName = "EZDrummer.zip";

    //        return File(fileStream, contentType, fileName, enableRangeProcessing: true);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, "Internal server error: " + ex.Message);
    //    }
    //}
    [Authorize]
    [HttpPut("{videoName}/revers")]
    public async Task<IActionResult> ReversVideo(string videoName, [FromForm] bool reverse)
    {
        try
        {

            var useridClaim = User.FindFirst("userid");

            if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
            {
                return StatusCode(402, "Not jwt token");
            }

            OptionsFfmpeg optionsFfmpeg = new OptionsFfmpeg
            {
                VideoName = videoName,
                UserIdClaim = int.Parse(useridClaim.Value),
                Revers = reverse,
            };

            if (optionsFfmpeg.Revers == true)
            {
                await _videoService.ReverseVideoAsync(optionsFfmpeg);
            }

            return Ok("File save");
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
    [HttpPut("{videoName}/adjust")]
    public async Task<IActionResult> AdjustSpeedVideoAsync(string videoName, [FromForm] double slowdownFactor)
    {
        try
        {
            string result = "";

            var useridClaim = User.FindFirst("userid");

            if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
            {
                return StatusCode(402, "Not jwt token");
            }

            OptionsFfmpeg optionsFfmpeg = new OptionsFfmpeg
            {
                VideoName = videoName,
                UserIdClaim = int.Parse(useridClaim.Value),
                SlowdownFactor = slowdownFactor
            };
            
            if (slowdownFactor > 1)
            {
                result = await _videoService.SpeedVideoAsync(optionsFfmpeg);
            }
            else if(slowdownFactor < 1)
            {
                result = await _videoService.SlowingVideoAsync(optionsFfmpeg);
            }

            if (result == null)
            {
                StatusCode(500, "Not file");
            }

            return Ok("File save");
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

    // PUT /api/videos/{videoName}/trim - обрезка видео по параметрам времени
    [Authorize]
    [HttpPut("{videoName}/trim")]
    public async Task<IActionResult> TrimVideo(string videoName, [FromForm] string startTime, [FromForm] string endTime)
    {
        try
        {
            var useridClaim = User.FindFirst("userid");

            if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
            {
                return StatusCode(402, "Not jwt token");
            }

            OptionsFfmpeg optionsFfmpeg = new OptionsFfmpeg
            {
                VideoName = videoName,
                UserIdClaim = int.Parse(useridClaim.Value),
                StartTime = startTime,
                EndTime = endTime
            };

            // Получаем обрезанное видео с помощью сервиса
            var result = await _videoService.TrimVideoAsync(optionsFfmpeg);

            if (result == null)
            {
                StatusCode(500, "Not file");
            }

            return Ok("File save");
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

    [HttpPost("save")]
    public async Task<IActionResult> SaveVideo([FromForm] IFormFile video)
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
    [HttpGet("GetVideo/{nameVideo}")]
    public async Task<IActionResult> GetVideo(string nameVideo)
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



    [HttpGet("check/{name}")]
    public IActionResult Check(string name)
    {
        try
        {
            return Ok(name);
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
