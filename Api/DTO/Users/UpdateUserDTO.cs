using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Api.DTO.Users;

public class UpdateUserDto
{
    [MaxLength(100)]
    public string UserName { get; set; }
    public string FullName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [Url]
    public string AvatarUrl { get; set; }
    [Description]
    public string Bio { get; set; }
    [Phone]
    public string Phone { get; set; }
    public string WorkAddress { get; set; }
    public string HomeAddress { get; set; }

}