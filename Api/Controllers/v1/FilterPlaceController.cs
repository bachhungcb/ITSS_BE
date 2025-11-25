using Application.Features.FilterPlaceFeatures.Commands;
using Application.Features.FilterPlaceFeatures.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class FilterPlaceController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(AddFilterPlaceCommand command)
    {
        try
        {
            var entityId = await Mediator.Send(command);
        return Ok(new { FilerPlaceId = entityId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid userId)
    {
        return Ok(await Mediator.Send(new GetUserFilterPlacesQuery(userId)));
    }
    
    
}