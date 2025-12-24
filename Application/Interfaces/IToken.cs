namespace Application.Interfaces;

public interface IToken
{
    string GenerateSafeRandomToken();
    public string GenerateOtp();
    string HashToken(string token);
}