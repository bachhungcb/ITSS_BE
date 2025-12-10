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
    
    [HttpGet("{osmId}")] // Ví dụ: api/v1/gym/node%2F123456 (cần url encode dấu /)
    public async Task<IActionResult> GetDetail([FromRoute] string osmId)
    {
        // Vì dấu "/" trong URL (node/123) có thể gây lỗi routing, 
        // bạn nên decode nó nếu client gửi dạng url-encoded, 
        // hoặc client gửi query param ?osmId=node/123 thì an toàn hơn.
        // Ở đây giả sử client gửi đúng format đã encode hoặc binding đúng.
        
        // Cách an toàn hơn là dùng Query Param nếu bạn không muốn config web server cho phép special characters trong path
        // Nhưng nếu dùng Route param, hãy đảm bảo client gọi api/v1/gym/node%2F12345
        
        try 
        {
            var osmIdDecoded = System.Net.WebUtility.UrlDecode(osmId);
            var result = await Mediator.Send(new GetGymDetailByIdQuery(osmIdDecoded));
            return Ok(new Api.Wrappers.Response<Domain.Entities.GymPlace>(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}