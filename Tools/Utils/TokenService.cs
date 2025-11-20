using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;

namespace Tools.Utils;

public class TokenService : IToken
{
    /// <summary>
    ///
    /// Create a random, long and unpredictable string
    /// </summary>
    /// 
    /// <returns>
    /// A random token
    /// </returns>
    public string GenerateSafeRandomToken()
    {
        //Create an 64-byte array
        var randomNumber = new byte[64];

        //Using RandomNumberGenerator to fill up the array
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        //Convert byte array to hex string
        //Hex string is 100% safe when being placed in URL

        return Convert.ToHexString(randomNumber);
    }
    
    public string HashToken(string token)
    {
        //Using SHA256 alg for hashing
        using (var sha256 = SHA256.Create())
        {
            //Convert token (strings) to byte array (using UTF-8)
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            //Calculating hash value
            var hashByte = sha256.ComputeHash(tokenBytes);

            //Convert hash to base64 string
            //Base64 string is shorter than Hex string
            return Convert.ToBase64String(hashByte);
        }
    }
}