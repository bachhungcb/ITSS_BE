using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using MediatR;

namespace Application.Features.AuthFeatures.Commands;

public class ResetPasswordCommand : IRequest<bool>
{
    // 1. Define input data
    public string Token { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    // 2. Define handler
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IToken _token;

        public ResetPasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IToken token
        )
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _token = token;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Confirm new password
            if (request.ConfirmPassword != request.Password)
            {
                throw new Exception("Passwords do not match.");
            }

            // 2. Hash received token
            var hashedToken = _token.HashToken(request.Token);

            // 3. Find user by hashed token
            var user = await _unitOfWork.UserRepository.FindByResetTokenAsync(hashedToken);

            // 4. Token validation
            if (user == null || user.ResetTokenExpires <= DateTime.UtcNow)
            {
                throw new Exception("Invalid token or token is expired.");
            }

            // 5. Update password
            var newHashPassword = _passwordHasher.HashPassword(request.Password);
            user.PasswordHash = newHashPassword;

            // 6. Disable token
            // Ensure that token is single use only
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            // 7. Save changes to DB
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}