using Api.DTO.Users;
using Api.Services;
using Application.Features.AuthFeatures.Commands;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class AuthController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IUriService _uriService;

    public AuthController(IUriService uriService, IMediator mediator)
    {
        _uriService = uriService;
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var token = await _mediator.Send(command);

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var userId = await _mediator.Send(command);

        return Ok(new { UserId = userId });
    }

    [HttpPost("reset-password-mail")]
    public async Task<IActionResult> SendResetPasswordMail([FromBody] ForgotPasswordDto dto)
    {
        // 1. Create command from dto
        var command = new SendResetPasswordMailCommand { Email = dto.Email };

        // 2. Send command to Handler
        await _mediator.Send(command);


        return Ok(new
        {
            Message = "We have send you a reset password email"
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        // 1. Create command from dto
        var command = new ResetPasswordCommand
        {
            Token = dto.Token,
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