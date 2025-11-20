using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.UserFeatures.Commands;

public record CreateUserCommand(
    string UserName,
    string Email,
    string FullName,
    string Password,
    string AvatarUrl,
    string Bio,
    string Phone) : IRequest<Guid>
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(
            IUnitOfWork unitOfWork, 
            IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Create hash password
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var createdAt = DateTime.UtcNow;
            
            // 2. Create Entity
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = hashedPassword,
                AvatarUrl = request.AvatarUrl,
                Bio = request.Bio,
                Phone = request.Phone,
                CreatedAt = createdAt,
            };
            
            // 3. Add to Repository
            _unitOfWork.UserRepository.Add(user);
            
            // 4. Save to DB
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // 5. Return Id
            return user.Id;
        }
    }
}