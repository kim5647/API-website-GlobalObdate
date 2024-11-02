using Microsoft.AspNetCore.Mvc;

namespace API_website.Controllers;


[ApiController]
[Route("api/user")]
public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
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

    [HttpPost("GetUserId")]
    public async Task<IActionResult> GetUserId([FromForm] int id)
    {
        var lll = await _userService.GetUserId(id);
        return Ok(lll);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginrUser([FromForm] string username, [FromForm] string password)
    {
        try
        {
            var token = await _userService.Login(username, password);

            Response.Cookies.Append("sex-bober", token);

            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    [HttpGet("hello")]
    public IActionResult Hellow()
    {
        return Ok("hellow word");
    }
    [HttpGet("get-userid")]
    public IActionResult GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst("userid");
        return Ok(userIdClaim.Value);
    }
}
