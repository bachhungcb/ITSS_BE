using Application.Features.UserFeatures.Commands;
using Application.Features.UserFeatures.Queries;
using MediatR;

namespace Application.Features.AuthFeatures.Commands;

public record RegisterCommand(
    string FullName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string AvatarUrl,
    string Bio,
    string Phone) : IRequest<Guid>
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IMediator _mediator;

        public RegisterCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new Exception("Register information can not be null");
            }

            if (request.ConfirmPassword != request.Password)
            {
                throw new Exception("Passwords do not match");
            }

            var existingUser = await _mediator.Send(new GetUserByEmailQuery(request.Email), cancellationToken);
            if (existingUser != null)
            {
                throw new Exception("Email already exists");
            }

            var userId = await _mediator.Send(new CreateUserCommand(request.UserName,
                request.Email,
                request.FullName,
                request.Password,
                request.AvatarUrl,
                request.Bio,
                request.Phone
            ), cancellationToken);

            return userId;
        }
    }
}