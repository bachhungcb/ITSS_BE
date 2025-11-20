using System.ComponentModel.DataAnnotations;

namespace Api.DTO.Users;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}