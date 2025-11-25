using Application.Features.GymFeatures.Queries;
using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class GymController : BaseApiController
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string address)
    {
        // Gọi Mediator
        var result = await Mediator.Send(new SearchGymsByAddressQuery(address));
        
        // Trả về kết quả chuẩn Response Wrapper
        return Ok(new Api.Wrappers.Response<IEnumerable<Domain.Entities.GymPlace>>(result));
    }
    
    [HttpGet("search-advanced")]
    public async Task<IActionResult> SearchAdvanced(
        [FromQuery] string originAddress, 
        [FromQuery] string? keyword, 
        [FromQuery] string? sortBy)
    {
        return Ok(new Api.Wrappers.Response<IEnumerable<GymPlace>>(
            await Mediator.Send(new SearchGymsAdvancedQuery(originAddress, keyword, sortBy))
        ));
    }
}