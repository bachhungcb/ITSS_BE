using Application.Interfaces;
using DevOne.Security.Cryptography.BCrypt;

namespace Tools.Utils;

public class PasswordHasher : IPasswordHasher
{
    private string Salt{get;set;}

    public PasswordHasher()
    {
        this.Salt = BCryptHelper.GenerateSalt();
    }
    public string HashPassword(string password)
    {
        return BCryptHelper.HashPassword(password, this.Salt);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        return BCryptHelper.CheckPassword(providedPassword, hashedPassword);
    }
}