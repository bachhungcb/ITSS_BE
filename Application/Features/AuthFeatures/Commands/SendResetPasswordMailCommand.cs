using Application.Features.UserFeatures.Queries;
using Application.Interfaces;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using Application.Features.UserFeatures.Commands;
using Microsoft.Extensions.Configuration;

namespace Application.Features.AuthFeatures.Commands;

public class SendResetPasswordMailCommand : IRequest<bool>
{
    public string Email { get; set; }

    public class ForgotPasswordCommandHandler : IRequestHandler<SendResetPasswordMailCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailSevice;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IToken _token;

        public ForgotPasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IMediator mediator,
            IConfiguration configuration,
            IToken token)
        {
            _unitOfWork = unitOfWork;
            _emailSevice = emailService;
            _mediator = mediator;
            _configuration = configuration;
            _token = token;
        }

        public async Task<bool> Handle(SendResetPasswordMailCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery(request.Email), cancellationToken);

            if (user is null)
            {
                //Do not throw error to combat with Email Enumeration
                return true;
            }

            // 1. Create a raw token
            string rawToken = _token.GenerateSafeRandomToken();

            // 2. Hash raw token
            string hashedToken = _token.HashToken(rawToken);

            // 3. Save hased token to DB            
            user.PasswordResetToken = hashedToken;
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15);
            
            // 4. Update user
            _unitOfWork.UserRepository.Update(user);
            
            // 5. Update database
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            //TODO: Implement resetLink based on FE URL
            var baseURL = _configuration.GetSection("BaseURL").Value;
            var resetLink = $"{baseURL}/reset-password?token={rawToken}";
            
            await _emailSevice.SendEmailAsync(
                user.Email,
                "Reset Password request",
                $"Please go to this link and reset your password: {resetLink}");
            return true;
        }
        
    }
}