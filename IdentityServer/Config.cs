using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [ 
            new IdentityResources.OpenId()
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

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // secret for authentication
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },

            // scopes that client has access to
            AllowedScopes = { "api1" }
        },
        new Client
            {
                ClientId = "webClient",
                RequireClientSecret = false,

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "http://localhost:5000/signin-oidc" },
                FrontChannelLogoutUri = "http://localhost:5000/signout-oidc",
                PostLogoutRedirectUris = { "http://localhost:5000/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "api1" }
            },
    ];
}