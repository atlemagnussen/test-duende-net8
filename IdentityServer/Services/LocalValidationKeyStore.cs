
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestDuende.IdentityServer.CryptoKeyStore;

namespace TestDuende.IdentityServer.Services;

public class LocalValidationKeyStore : IValidationKeysStore
{
    private readonly CryptoKeys _keys;

    public LocalValidationKeyStore(IOptions<CryptoKeys> opts)
    {
        _keys = opts.Value;
    }

    private SecurityKeyInfo GetSecurityKeyInfo(Microsoft.IdentityModel.Tokens.JsonWebKey jwk)
    {
        SecurityKey key = CryptoService.GetSecurityKeyFromJwk(_keys.Active);
        var keyInfo = new SecurityKeyInfo { Key = key, SigningAlgorithm = _keys.Active.Alg };
        return keyInfo;
    }

    public Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
    {
        var list = new List<SecurityKeyInfo>
        {
            GetSecurityKeyInfo(_keys.Active)
        };

        if (_keys.Others is not null)
        {
            foreach (var other in _keys.Others) {
                list.Add(GetSecurityKeyInfo(other));
            }
        }
        return Task.FromResult<IEnumerable<SecurityKeyInfo>>(list);
    }
}