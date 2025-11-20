using Api.DTO.Users;
using Api.Filter;
using Api.Helpers;
using Api.Services;
using Api.Wrappers;
using Application.Features.AuthFeatures.Commands;
using Application.Features.UserFeatures.Commands;
using Application.Features.UserFeatures.Queries;
using Asp.Versioning;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class UserController : BaseApiController
{
    private readonly IUriService _uriService;
    private readonly IMediator _mediator;

    public UserController(IUriService uriService, IMediator mediator)
    {
        _uriService = uriService;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        return Ok(new Response<Guid>(await _mediator.Send(command)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationFilter filter)
    {
        var route = Request.Path.Value;
        var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
        var response = (await _mediator.Send(new GetAllUserQuery(validFilter.PageNumber, validFilter.PageSize)))
            .ToList();
        var totalRecords = response.Count;
        var returnResponse =
            PaginationHelper.CreatePagedReponse<User>(  response,
                                                        validFilter,
                                                        totalRecords,
                                                        _uriService,
                                                        route);
        return Ok(returnResponse);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        return Ok(await _mediator.Send(new GetUserByIdQuery(id)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        return Ok(await _mediator.Send(new DeleteUserByIdCommand { Id = id }));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser([FromRoute]Guid id,[FromBody]UpdateUserDto dto)
    {
        var command = new UpdateUserCommand
        {
            Id = id,
            UserName = dto.UserName,
            FullName = dto.FullName,
            Email = dto.Email,
            AvatarUrl = dto.AvatarUrl,
            Bio = dto.Bio,
            Phone = dto.Phone
        };
        return Ok(await _mediator.Send(command));
    }

   
}