using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserFeatures.Commands;

public class UpdateUserCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public string Bio { get; set; }
    public string Phone { get; set; }

    
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetById(request.Id);
            var updatedAt = DateTime.UtcNow;
            if (user == null)
            {
                return Guid.Empty;
            }
            else
            {
                user.FullName = request.FullName;
                user.Email = request.Email;
                user.UserName = request.UserName;
                user.AvatarUrl = request.AvatarUrl;
                user.Bio = request.Bio;
                user.Phone = request.Phone;
                user.UpdatedAt = updatedAt;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return user.Id;
            }
        }
    }
}