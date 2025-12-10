using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.GymFeatures.Queries;

public record GetGymDetailByIdQuery(string OsmId) : IRequest<GymPlace>
{
    public class Handler : IRequestHandler<GetGymDetailByIdQuery, GymPlace>
    {
        private readonly IGymSearchService _gymSearchService;

        public Handler(IGymSearchService gymSearchService)
        {
            _gymSearchService = gymSearchService;
        }

        public async Task<GymPlace> Handle(GetGymDetailByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.OsmId))
            {
                throw new ArgumentException("OsmId cannot be empty");
            }
            
            var gym = await _gymSearchService.GetGymPlaceByOsmIdAsync(request.OsmId);
            
            if (gym == null)
            {
                // Có thể throw NotFoundException để Controller bắt lỗi 404
                throw new KeyNotFoundException($"Gym with OsmId {request.OsmId} not found");
            }

            return gym;
        }
    }
}