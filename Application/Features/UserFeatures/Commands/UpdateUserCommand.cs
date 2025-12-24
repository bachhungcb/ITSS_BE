using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserFeatures.Commands;

public class UpdateUserCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    [DisplayName("User Name")]
    public string UserName { get; set; }
    [DisplayName("Full Name")]
    public string FullName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [Url]
    public string AvatarUrl { get; set; }
    [Description]
    public string Bio { get; set; }
    [Phone]
    public string Phone { get; set; }
    public string WorkAddress { get; set; }
    public string HomeAddress { get; set; }
    
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
                user.WorkAddress = request.WorkAddress;
                user.HomeAddress = request.HomeAddress;
                user.UpdatedAt = updatedAt;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return user.Id;
            }
        }
    }
}