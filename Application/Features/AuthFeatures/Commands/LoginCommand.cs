using Application.Features.UserFeatures.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Features.AuthFeatures.Commands;

public class LoginCommand : IRequest<string>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(IMediator mediator, IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _mediator = mediator;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery(request.Email), cancellationToken);

            if (user is null)
            {
                throw new Exception("User not found");
            }

            var isPasswordValid = _passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password);

            if (!isPasswordValid)
            {
                throw new Exception("Invalid password");
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            return token;
        }
    }
}