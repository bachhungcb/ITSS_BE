using Api.DTO.Gyms;
using Api.Wrappers;
using Application.Features.GymFeatures.Queries;
using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Tools.Services;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class GymController : BaseApiController
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string address)
    {
        // 1. Gọi Service lấy dữ liệu Entity chuẩn
        var gymPlaces = await Mediator.Send(new SearchGymsByAddressQuery(address));
    
        // 2. Chuyển đổi (Map) Entity sang DTO để trả về đúng JSON mong muốn
        var response = gymPlaces.Select(g => new GymResponse
        {
            Id = g.OsmId ?? g.Id.ToString(), // Dùng OsmId hoặc Guid
            Name = g.Name,
            ImageUrl = g.ImageUrl,
        
            // Tạo cấu trúc lồng nhau cho Sports
            Sports = new SportInfo 
            { 
                Tags = g.Sports 
            },
        
            // Tạo cấu trúc lồng nhau cho Location
            Location = new LocationInfo
            {
                Address = g.Address,
                Coordinates = new Coordinates
                {
                    Lat = g.Latitude,
                    Lng = g.Longitude
                }
            }
        }).ToList();

        // 3. Trả về
        return Ok(new Api.Wrappers.Response<List<GymResponse>>(response));
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