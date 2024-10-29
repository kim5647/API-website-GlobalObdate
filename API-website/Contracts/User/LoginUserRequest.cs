
using System.ComponentModel.DataAnnotations;

public record LoginUserRequest(
    [Required] string Username,
    [Required] string Password);