using Application.Interfaces;
using MediatR;

namespace Application.Features.AuthFeatures.Commands;

public class ChangePasswordCommand : IRequest<bool>
{
    // 1. Define input data
    public Guid Id { get; set; } //User ID
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }

    // 2. Define handler
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Confirm new password
            if (request.ConfirmNewPassword != request.NewPassword)
            {
                throw new Exception("Password do NOT match");
            }

            // 2. Find user by Id
            var user = await _unitOfWork.UserRepository.GetById(request.Id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // 2.1. Validate old password
            var correctOldPassword = _passwordHasher.VerifyHashedPassword(
                user.PasswordHash,
                request.OldPassword);

            if (!correctOldPassword)
            {
                throw new Exception("Old password do NOT match");
            }

            // 3. Update password
            var newHashPassword = _passwordHasher.HashPassword(request.NewPassword);
            user.PasswordHash = newHashPassword;

            // 4. Save changes to DB
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}