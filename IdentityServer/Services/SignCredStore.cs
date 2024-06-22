using Duende.IdentityServer.Stores;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestDuende.IdentityServer.CryptoKeyStore;

namespace TestDuende.IdentityServer.Services;

public class SignCredStore : ISigningCredentialStore
{
    private readonly CryptoKeys _keys;

    public SignCredStore(IOptions<CryptoKeys> opts)
    {
        _keys = opts.Value;
    }
    public Task<SigningCredentials> GetSigningCredentialsAsync()
    {
        SecurityKey active = CryptoService.GetSecurityKeyFromJwk(_keys.Active);
        string algorithm = _keys.Active.Alg;
        var cred = new SigningCredentials(active, algorithm);
        return Task.FromResult(cred);
    }
}