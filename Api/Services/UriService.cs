using Api.Filter;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Services;

public class UriService : IUriService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UriService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Uri GetPageUri(PaginationFilter filter, string route)
    {
        // Lấy request từ context của request hiện tại, không phải request đầu tiên
        var request = _httpContextAccessor.HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host.ToUriComponent()}";
        var endpointUri = new Uri(string.Concat(baseUrl, route));

        // ... logic tạo URL đầy đủ với query parameters ...
        // Ví dụ:
        var modifiedUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
        modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());

        return new Uri(modifiedUri);
    }
}