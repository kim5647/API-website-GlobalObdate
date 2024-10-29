
using System.ComponentModel.DataAnnotations;

public record RegisterUserRequest(
    [Required] string Username,
    [Required] string Password);