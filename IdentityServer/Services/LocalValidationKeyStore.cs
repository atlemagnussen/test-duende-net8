
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestDuende.IdentityServer.CryptoKeyStore;

namespace TestDuende.IdentityServer.Services;

public class LocalValidationKeyStore : IValidationKeysStore
{
    private readonly CryptoKeys _keys;
    private readonly ILogger<LocalValidationKeyStore> _logger;

    public LocalValidationKeyStore(IOptions<CryptoKeys> opts,
        ILogger<LocalValidationKeyStore> logger)
    {
        _keys = opts.Value;
        _logger = logger;
    }

    private SecurityKeyInfo GetSecurityKeyInfo(Microsoft.IdentityModel.Tokens.JsonWebKey jwk)
    {
        SecurityKey key = CryptoService.GetSecurityKeyFromJwk(jwk);
        var keyInfo = new SecurityKeyInfo { Key = key, SigningAlgorithm = jwk.Alg };
        return keyInfo;
    }

    public Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
    {
        _logger.LogInformation("Active {0}", _keys.Active.Kid);
        var list = new List<SecurityKeyInfo>
        {
            GetSecurityKeyInfo(_keys.Active)
        };

        _logger.LogInformation("Other count {0}", _keys.Others.Count);
        if (_keys.Others is not null)
        {
            foreach (var other in _keys.Others)
            {
                _logger.LogInformation("Other {0}", other.Kid);
                list.Add(GetSecurityKeyInfo(other));
            }
        }
        return Task.FromResult<IEnumerable<SecurityKeyInfo>>(list);
    }
}