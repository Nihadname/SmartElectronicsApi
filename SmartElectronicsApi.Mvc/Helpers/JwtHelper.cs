using System.IdentityModel.Tokens.Jwt;

namespace SmartElectronicsApi.Mvc.Helpers
{
    public static class JwtHelper
    {
        public static string GetUserIdFromJwt(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken11 = handler.ReadJwtToken(jwtToken);
            var userIdClaim = jwtToken11.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;

            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User ID claim not found in the JWT token.");
            }

            return userIdClaim;
        }
    }
}
