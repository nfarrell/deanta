using BoafyAuth.Admin.Configuration.Constants;
using BoafyAuth.Admin.Configuration.Interfaces;

namespace BoafyAuth.Admin.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "https://admin.boafy.com";
        public string IdentityAdminRedirectUri { get; set; } = "https://admin.boafy.com/signin-oidc";
        public string IdentityServerBaseUrl { get; set; } = "https://id.boafy.com";
        public string ClientId { get; set; } = AuthenticationConsts.OidcClientId;
        public string[] Scopes { get; set; }
        public string IdentityAdminApiSwaggerUIClientId { get; } = AuthenticationConsts.IdentityAdminApiSwaggerClientId;
        public string IdentityAdminApiSwaggerUIRedirectUrl { get; } =
            "http://localhost:5001/swagger/oauth2-redirect.html";
        public string IdentityAdminApiScope { get; } = AuthenticationConsts.IdentityAdminApiScope;
        public string ClientSecret { get; set; } = AuthenticationConsts.OidcClientSecret;
        public string OidcResponseType { get; set; } = AuthenticationConsts.OidcResponseType;
    }
}
