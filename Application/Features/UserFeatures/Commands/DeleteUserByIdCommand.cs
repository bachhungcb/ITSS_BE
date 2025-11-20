using System.Diagnostics;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserFeatures.Commands;

public class DeleteUserByIdCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public class DeleteUserByIdCommandHandler : IRequestHandler<DeleteUserByIdCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetById(request.Id);
            if (user == null) return Guid.Empty;
            _unitOfWork.UserRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}