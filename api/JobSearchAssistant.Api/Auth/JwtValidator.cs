

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace JobSearchAssistant.Api.Auth
{
    public sealed class JwtValidator
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configManager;

        public JwtValidator(IConfiguration config)
        {
            var domain = config["Auth0:Domain"]?? throw new InvalidOperationException("Auth0:Domain missing (check local.settings.json Values)");
            _audience = config["Auth0:Audience"]?? throw new InvalidOperationException("Auth0:Audience missing");
            _issuer = $"https://{domain}/";

            var metadataAddress = $"{_issuer}.well-known/openid-configuration";
            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataAddress,
                new OpenIdConnectConfigurationRetriever()
            );
        }
        public async Task<ClaimsPrincipal> ValidateAsync(string token, CancellationToken ct)
        {
            var oidc = await _configManager.GetConfigurationAsync(ct);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,

                ValidateAudience = true,
                ValidAudience = _audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = oidc.SigningKeys,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, parameters,out _);
        }
    }
}