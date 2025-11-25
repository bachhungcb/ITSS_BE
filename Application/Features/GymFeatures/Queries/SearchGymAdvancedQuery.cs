using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.GymFeatures.Queries;

public record SearchGymsAdvancedQuery(string Address, string? Keyword, string? SortBy) : IRequest<IEnumerable<GymPlace>>
{
    public class Handler : IRequestHandler<SearchGymsAdvancedQuery, IEnumerable<GymPlace>>
    {
        private readonly IGymSearchService _gymSearchService;

        public Handler(IGymSearchService gymSearchService)
        {
            _gymSearchService = gymSearchService;
        }

        public async Task<IEnumerable<GymPlace>> Handle(SearchGymsAdvancedQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy dữ liệu thô từ OSM (đã có tính distance ở Bước 2)
            var gyms = await _gymSearchService.SearchGymsAroundAddressAsync(request.Address);

            // 2. Lọc theo từ khóa (Keyword)
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                gyms = gyms.Where(g => 
                    (g.Name != null && g.Name.ToLower().Contains(keyword)) ||
                    (g.Address != null && g.Address.ToLower().Contains(keyword))
                ).ToList();
            }

            // 3. Sắp xếp (Sort)
            if (request.SortBy == "distance_asc") // Gần nhất
            {
                gyms = gyms.OrderBy(g => g.DistanceInMeters).ToList();
            }

            return gyms;
        }
    }
}