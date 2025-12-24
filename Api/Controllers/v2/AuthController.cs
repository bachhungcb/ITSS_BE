using Api.DTO.Users;
using Api.Services;
using Application.Features.AuthFeatures.Commands;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v2;

[ApiVersion("2.0")]
public class AuthController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IUriService _uriService;

    public AuthController(IUriService uriService, IMediator mediator)
    {
        _uriService = uriService;
        _mediator = mediator;
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        // 1. Create command from dto
        var command = new ResetPasswordCommand
        {
            Token = "123456",
            Password = dto.Password,
            ConfirmPassword = dto.ConfirmPassword,
        };

        // 2. Send command and handle exception
        try
        {
            await _mediator.Send(command);
            return Ok(new
            {
                message = "Reset password successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        // 1. Create command from dto
        var command = new ChangePasswordCommand
        {
            Id = dto.Id,
            OldPassword = dto.OldPassword,
            NewPassword = dto.NewPassword,
            ConfirmNewPassword = dto.ConfirmNewPassword
        };
        
        // 2. Send command and handle exception
        try
        {
            await _mediator.Send(command);
            return Ok(new
            {
                message = "Change password succesfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new{message = ex.Message});
        }
    }
    
}