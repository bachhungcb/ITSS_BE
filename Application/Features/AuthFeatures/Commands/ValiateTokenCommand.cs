using Application.Interfaces;
using MediatR;

namespace Application.Features.AuthFeatures.Commands;

public record ValidateTokenCommand(string Token) : IRequest<bool>
{
    public class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToken _token;

        public ValidateTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IToken token
        )
        {
            _unitOfWork = unitOfWork;
            _token = token;
        }

        public async Task<bool> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
        {
            // 2. Hash received token
            var hashedToken = _token.HashToken(request.Token);

            // 3. Find user by hashed token
            var user = await _unitOfWork.UserRepository.FindByResetTokenAsync(hashedToken);

            // 4. Token validation
            if (user == null || user.ResetTokenExpires <= DateTime.UtcNow)
            {
                throw new Exception("Invalid token or token is expired.");
            }
        
            return true;
        }
    }
}

