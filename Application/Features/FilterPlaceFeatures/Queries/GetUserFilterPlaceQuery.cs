using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.FilterPlaceFeatures.Queries;

public record GetUserFilterPlacesQuery(Guid UserId) : IRequest<IEnumerable<FilterPlace>>
{
    public class Handler : IRequestHandler<GetUserFilterPlacesQuery, IEnumerable<FilterPlace>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FilterPlace>> Handle(GetUserFilterPlacesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FilterPlaceRepository.GetByUserId(request.UserId);
        }
    }
}