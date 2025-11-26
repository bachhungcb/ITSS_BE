using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.FilterPlaceFeatures.Commands;

public record AddFilterPlaceCommand(Guid UserId, string Name, string Address) : IRequest<IEnumerable<FilterPlace>>
{
    // 2. Sửa kiểu trả về của Handler
    public class Handler : IRequestHandler<AddFilterPlaceCommand, IEnumerable<FilterPlace>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FilterPlace>> Handle(AddFilterPlaceCommand request, CancellationToken cancellationToken)
        {
            var entity = new FilterPlace
            {
                UserId = request.UserId,
                Name = request.Name,
                Address = request.Address
            };

            // Thêm vào DbContext (chưa lưu)
            _unitOfWork.FilterPlaceRepository.Add(entity);
            
            // Lưu xuống DB
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // 3. Lấy lại danh sách mới nhất để trả về
            // (Tận dụng hàm GetByUserId đã có trong Repository)
            var updatedList = await _unitOfWork.FilterPlaceRepository.GetByUserId(request.UserId);

            return updatedList;
        }
    }
}