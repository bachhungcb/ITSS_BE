using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtTokenGenerator
{
    public string GenerateToken(User user);
}