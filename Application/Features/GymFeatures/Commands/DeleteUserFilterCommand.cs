namespace Application.Features.GymFeatures.Commands
{
    public record DeleteUserFilterCommand(Guid UserId) : IRequest
    {
        public class DeleteUserFilterCommandHandler : IRequestHandler<DeleteUserFilterCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteUserFilterCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(DeleteUserFilterCommand request, CancellationToken cancellationToken)
            {
                var filters = await _unitOfWork.FilterPlaceRepository.GetFiltersByUserIdAsync(request.UserId);
                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        _unitOfWork.FilterPlaceRepository.Remove(filter);
                    }
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}