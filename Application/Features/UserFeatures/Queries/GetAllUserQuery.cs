using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserFeatures.Queries;

public record GetAllUserQuery(int PageNumber, int PageSize) : IRequest<IEnumerable<User>>
{
    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, IEnumerable<User>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUserQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> Handle(GetAllUserQuery query, CancellationToken cancellationToken)
        {
            int pageSize = query.PageSize;
            int pageNumber = query.PageNumber;

            // 1. Get Userquery from repository
            var userQuery = _unitOfWork.UserRepository.GetAllAsQueryable();

            // 2. Concat Skip and Take
            // EF Core will make this into one SQL query

            var userList = await userQuery
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken: cancellationToken);
            
            if (userList == null)
            {
                return null;
            }

            return userList.AsReadOnly();
        }
    }
}