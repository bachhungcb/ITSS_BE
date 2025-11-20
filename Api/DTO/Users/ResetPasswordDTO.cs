using System.ComponentModel.DataAnnotations;

namespace Api.DTO.Users;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}