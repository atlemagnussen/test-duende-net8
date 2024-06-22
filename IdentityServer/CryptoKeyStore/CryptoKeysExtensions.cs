using Microsoft.IdentityModel.Tokens;

namespace TestDuende.IdentityServer.CryptoKeyStore;

public static class ModelServiceExtensions
{
    public static void AddOptionsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // CryptoKeys cryptoKeys = new CryptoKeys(active, keys);
        services.Configure<CryptoKeys>(ck => {
            var section = configuration.GetSection("CryptoKeys");
            CryptoKeysConfig config = new();
            section.Bind(config);
            var keys = config.Keys.ToList();
            foreach (var key in keys) {
                if (key.Kty == "EC") {
                    SetCorrectAlgorithmForEcKey(key);
                }
            }
            var active = keys.Find(k => k.Kid == config.ActiveKey) ?? throw new ApplicationException($"no active key found by name {config.ActiveKey}");
            keys.Remove(active);
            ck.Active = active;
            ck.Others = keys;
        });
    }

    public static void SetCorrectAlgorithmForEcKey(JsonWebKey key)
    {
        var hashAlg = key.Crv switch
        {
            "P-256" => SecurityAlgorithms.EcdsaSha256,
            "P-384" => SecurityAlgorithms.EcdsaSha384,
            "P-521" => SecurityAlgorithms.EcdsaSha512,
            _ => throw new NotSupportedException()
        };
        key.Alg = hashAlg;
    }
    
    // public static void AddAuthenticationServices(this IServiceCollection services)
    // {
    //     services.AddSingleton<GetCryptoKeys>();
    // }
}