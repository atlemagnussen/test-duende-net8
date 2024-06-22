using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace TestDuende.IdentityServer.CryptoKeyStore;

public record CryptoKeysConfig
{
    public string ActiveKey { get; init; } = "key1";

    public IEnumerable<JsonWebKey> Keys { get; init; } = [];
}

public record CryptoKeys
{
    public CryptoKeys()
    {
        Active = new JsonWebKey();
        Others = new List<JsonWebKey>();
    }
    public CryptoKeys(JsonWebKey active, List<JsonWebKey> others)
    {
        Active = active;
        Others = others;
    }
    public JsonWebKey Active { get; set; }
    public List<JsonWebKey> Others { get; set; }

    public override string ToString()
    {
        var activeStr = JsonSerializer.Serialize(Active);
        var othersStr = JsonSerializer.Serialize(Others);

        return $"active: {activeStr}, others: {othersStr}";
    }
}

public class GetCryptoKeys
{
    private readonly CryptoKeysConfig _config;
    public GetCryptoKeys(IConfiguration configuration)
    {
        var section = configuration.GetSection("CryptoKeys");
        CryptoKeysConfig config = new();
        section.Bind(config);

        if (config == null)
            throw new ApplicationException("missing keys");
        _config = config;
    }
    public CryptoKeys FromConfig()
    {
        var keys = _config.Keys.ToList();
        var active = keys.Find(k => k.Kid == _config.ActiveKey);
        if (active == null)
            throw new ApplicationException($"no active key found by name {_config.ActiveKey}");
        
        keys.Remove(active);
        return new CryptoKeys(active, keys);
    }
}