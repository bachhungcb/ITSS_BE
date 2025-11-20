namespace Application.Interfaces;

public interface IToken
{
    string GenerateSafeRandomToken();

    string HashToken(string token);
}