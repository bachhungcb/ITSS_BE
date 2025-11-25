using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.FilterPlaceFeatures.Commands;

public record AddFilterPlaceCommand(Guid UserId, string Name, string Address) : IRequest<Guid>
{
    public class Handler : IRequestHandler<AddFilterPlaceCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        // Có thể inject thêm IGymSearchService nếu muốn lấy tọa độ từ Address ngay lúc tạo

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AddFilterPlaceCommand request, CancellationToken cancellationToken)
        {
            var entity = new FilterPlace
            {
                UserId = request.UserId,
                Name = request.Name,
                Address = request.Address
                // Latitude/Longitude: Có thể gọi service để lấy tọa độ tại đây
            };

            _unitOfWork.FilterPlaceRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}