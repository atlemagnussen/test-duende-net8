using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
                Name = "roles",
                DisplayName = "Roles",
                Description = "Allow the service access to your user roles.",
                UserClaims = [ JwtClaimTypes.Role ],
                ShowInDiscoveryDocument = true,
                Required = true,
                Emphasize = true
            }
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope(name: "api1", displayName: "My API")
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "apiClient",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "api1" }
        },
        new Client
            {
                ClientId = "webClient",
                RequireClientSecret = false,
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                EnableLocalLogin = true,

                AllowedCorsOrigins = ["http://localhost:5000/callback.html"],
                RedirectUris = [ "http://localhost:5000/signin-oidc" ],
                FrontChannelLogoutUri = "http://localhost:5000/signout-oidc",
                PostLogoutRedirectUris = [ "http://localhost:5000/signout-callback-oidc" ],

                AccessTokenLifetime = 3600,
                AllowOfflineAccess = true,
                AuthorizationCodeLifetime = 300,
                AccessTokenType = AccessTokenType.Jwt,
                AllowedScopes = [
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "api1"
                ]
            },
    ];
}