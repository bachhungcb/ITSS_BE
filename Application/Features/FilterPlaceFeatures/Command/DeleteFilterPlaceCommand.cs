using Application.Interfaces;
using MediatR;

namespace Application.Features.FilterPlaceFeatures.Commands;

public class DeleteFilterPlaceCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public class DeleteFilterPlaceCommandHandler : IRequestHandler<DeleteFilterPlaceCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFilterPlaceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(DeleteFilterPlaceCommand request, CancellationToken cancellationToken)
        {
            var filterPlace = await _unitOfWork.FilterPlaceRepository.GetById(request.Id);
            if (filterPlace == null) return Guid.Empty;
            _unitOfWork.FilterPlaceRepository.Remove(filterPlace);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return filterPlace.Id;
        }
    }
}