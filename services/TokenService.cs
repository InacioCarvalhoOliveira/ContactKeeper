using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ContactKeeper.Models;

namespace ContactKeeper.Services
{
    public class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    //new Claim(ClaimTypes.Role, user.Password.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "YourIssuer",
                Audience = "YourAudience"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
     
        public static string ValidateToken(string token)
    {
        if (TokenBlacklist.IsBlacklisted(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = "YourIssuer",
                ValidateAudience = true,
                ValidAudience = "YourAudience",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null)
                throw new SecurityTokenException("Invalid token");

            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "unique_name");
            if (userIdClaim == null)
                throw new SecurityTokenException("Invalid token: missing user ID claim.");

        return userIdClaim.Value;

        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (Exception)
        {
            throw; // Rethrow the exception to be handled elsewhere
        }
        }
    }
    public static class TokenBlacklist
    {
        private static readonly HashSet<string> BlacklistedTokens = new HashSet<string>();

        public static void AddToBlacklist(string token)
        {
            BlacklistedTokens.Add(token.ToLowerInvariant());
        }

        public static bool IsBlacklisted(string token)
        {
            return BlacklistedTokens.Contains(token.ToLowerInvariant());
        }
    }
}