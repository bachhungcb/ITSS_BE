using System.ComponentModel.DataAnnotations;

namespace Api.DTO.Users;

public class UpdateUserDto
{
    [MaxLength(100)]
    public string UserName { get; set; }
    public string FullName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public string Bio { get; set; }
    public string Phone { get; set; }

}