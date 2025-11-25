using Application.Interfaces;
using Domain.Entities;

namespace Domain.Interfaces;

// Lưu ý: Đảm bảo IGenericRepository đến từ Application.Interfaces
public interface IFilterPlaceRepository : IGenericRepository<FilterPlace>
{
    // Đổi tên hàm cho khớp với class thực thi
    Task<IEnumerable<FilterPlace>> GetByUserId(Guid userId);
}