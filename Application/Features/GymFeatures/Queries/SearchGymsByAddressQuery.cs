using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.GymFeatures.Queries;

public record SearchGymsByAddressQuery(string Address) : IRequest<IEnumerable<GymPlace>>
{
    public class SearchGymsByAddressHandler : IRequestHandler<SearchGymsByAddressQuery, IEnumerable<GymPlace>>
    {
        private readonly IGymSearchService _gymSearchService;

        public SearchGymsByAddressHandler(IGymSearchService gymSearchService)
        {
            _gymSearchService = gymSearchService;
        }

        public async Task<IEnumerable<GymPlace>> Handle(SearchGymsByAddressQuery request, CancellationToken cancellationToken)
        {
            // Validate input tại đây nếu cần (Validation Layer)
            if (string.IsNullOrWhiteSpace(request.Address))
            {
                throw new ArgumentException("Address cannot be empty");
            }

            // Gọi service hạ tầng
            var gyms = await _gymSearchService.SearchGymsAroundAddressAsync(request.Address);
            
            return gyms;
        }
    }
}