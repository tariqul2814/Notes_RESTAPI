using InsightRESTAPI.Common;
using InsightRESTAPI.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InsightRESTAPI.Services.Admin
{
    public class JWTServices
    {
        public JWTServices(IConfiguration configuration, JWTSettingsConfig jWTSettingsConfig)
        {
            Configuration = configuration;
            _jWTSettingsConfig = jWTSettingsConfig;
            Key = Encoding.ASCII.GetBytes(_jWTSettingsConfig.Secret);
            ExpiresInMinutes = _jWTSettingsConfig.ExpiresInMinutes;
        }

        private IConfiguration Configuration { get; }
        private readonly JWTSettingsConfig _jWTSettingsConfig;
        private byte[] Key { get; }
        private DateTime Expires { get; }
        private int ExpiresInMinutes { get; }

        #region Private Methods
        private string GetTokenFor(string userId, string userRole, int expiresInMinutes, byte[] key)
        {
            var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.Role,userRole),
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var output = tokenHandler.WriteToken(token);
            return output;
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token, byte[] key)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        private string GetSignatureToken(byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var output = tokenHandler.WriteToken(token);
            return output;
        }
        private bool ValidateSignature(byte[] key, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                }, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// overload method load configuration from app settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRole"></param>
        /// <returns>JWT token</returns>
        public string GetTokenFor(string userId, string userRole)
        {
            return GetTokenFor(userId, userRole, ExpiresInMinutes, Key);
        }

        /// <summary>
        /// Get The Principal from token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            return GetPrincipalFromExpiredToken(token, Key);
        }

        /// <summary>
        /// Get The Server Signature Token
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public string GetServerSignatureToken(string serverName)
        {
            var key = Encoding.ASCII.GetBytes(Utilities.ResolveServerName(serverName));
            return GetSignatureToken(key);
        }

        /// <summary>
        /// Validate the server token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public bool ValidateServerSignature(string token, string serverName)
        {
            var key = Encoding.ASCII.GetBytes(Utilities.ResolveServerName(serverName));
            return ValidateSignature(key, token);
        }


        /// <summary>
        /// Ge tWebhook Signature Token
        /// </summary>
        /// <param name="storeCode"></param>
        /// <returns></returns>
        public string GetWebhookSignatureToken(string storeCode)
        {
            var key = Encoding.ASCII.GetBytes(Utilities.ResolveServerName(storeCode));
            return GetSignatureToken(key);
        }

        /// <summary>
        /// Validate Webhook Signature
        /// </summary>
        /// <param name="token"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public bool ValidateWebhookSignature(string token, string storeCode)
        {
            var key = Encoding.ASCII.GetBytes(Utilities.ResolveServerName(storeCode));
            return ValidateSignature(key, token);
        }
    }
}
