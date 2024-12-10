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
            else if (slowdownFactor < 1)
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

    [HttpPost("{nameVideo}/save")]
    public async Task<IActionResult> SaveVideo(string nameVideo, [FromForm] IFormFile video, [FromForm] string smile)
    {
        try
        {
            var useridClaim = User.FindFirst("userid");

            if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
            {
                return StatusCode(402, "No JWT token");
            }

            if (!int.TryParse(useridClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID format in token");
            }

            var filePath = await _videoService.SaveVideoAsync(video, nameVideo, smile, userId);

            return Ok("Video saved successfully at path: " + filePath);
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
    [HttpPut("{nameVideo}/addClip")]
    public async Task<IActionResult> AddClipAsync(string nameVideo, [FromForm] IFormFile video, [FromForm] bool point)
    {
        var useridClaim = User.FindFirst("userid");

        if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
        {
            return StatusCode(402, "No JWT token");
        }

        await _videoService.AddClipAsync(nameVideo, video, int.Parse(useridClaim.Value), point);

        return Ok();
    }

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

    [HttpGet("GetAllVideoInfo")]
    public async Task<IActionResult> GetAllVideoInfo()
    {
        var useridClaim = User.FindFirst("userid");

        if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
        {
            return StatusCode(402, "Not jwt token");
        }

        List<VideoInfo> values = await _videoService.GetAllVideo(int.Parse(useridClaim.Value));

        return Ok(values);
    }
    [HttpDelete("DeleteVideo/{videoName}")]
    public async Task<IActionResult> DeleteVideoAsync(string videoName)
    {
        try
        {
            var useridClaim = User.FindFirst("userid");

            if (useridClaim == null || string.IsNullOrEmpty(useridClaim.Value))
            {
                return StatusCode(402, "No JWT token");
            }

            await _videoService.DeleteVideoAsync(videoName, int.Parse(useridClaim.Value));

            return Ok("Delete video");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}
