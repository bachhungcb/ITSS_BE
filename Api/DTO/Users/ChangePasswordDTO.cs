using System.ComponentModel.DataAnnotations;

namespace Api.DTO.Users;

public class ChangePasswordDTO
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string OldPassword { get; set; }
    
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
    
    
    [Required]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
}