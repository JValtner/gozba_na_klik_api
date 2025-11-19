using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Gozba_na_klik.Utils
{
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Izvlači UserId iz JWT token claims
        /// </summary>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // Prvo probaj "userid" claim (custom claim)
            var userIdClaim = user.FindFirst("userid")?.Value;
            
            // Ako ne postoji, probaj standardni "sub" claim
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            }

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }

        /// <summary>
        /// Izvlači Username iz JWT token claims
        /// </summary>
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst("username")?.Value 
                ?? user.FindFirst(ClaimTypes.Name)?.Value 
                ?? throw new UnauthorizedAccessException("Username not found in token.");
        }
    }
}

