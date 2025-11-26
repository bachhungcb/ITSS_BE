using MediatR;

namespace Application.Features.GymFeatures.Commands;

public record AddUserFilterCommand(
    Guid UserId,
    string Name,
    string Address) : IRequest
{
    public class AddUserFilterCommandHandler : IRequestHandler<AddUserFilterCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUserFilterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddUserFilterCommand request, CancellationToken cancellationToken)
        {
            var filterPlace = new Domain.Entities.FilterPlace
            {
                UserId = request.UserId,
                Name = request.Name,
                Address = request.Address
            };

            await _unitOfWork.FilterPlaceRepository.AddFilterPlaceAsync(filterPlace);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}