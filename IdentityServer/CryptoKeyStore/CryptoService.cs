using Microsoft.IdentityModel.Tokens;

namespace TestDuende.IdentityServer.CryptoKeyStore;

public static class CryptoService
{
    /// <summary>
    /// Sign a JWT
    /// </summary>
    /// <param name="jwt">Must be header.payload</param>
    /// <param name="jwk">Must be private key</param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static string? SignJwk(string jwt, JsonWebKey jwk)
    {
        
        IKeyService keyService = GetKeyService(jwk);
        return keyService.SignJwt(jwt);
    }

    public static SecurityKey GetSecurityKeyFromJwk(JsonWebKey jwk)
    {
        IKeyService keyService = GetKeyService(jwk);
        return keyService.GetSecurityKeyFromJwk();
    }

    public static bool Verify(string data, string signature, JsonWebKey jwk)
    {
        IKeyService keyService = GetKeyService(jwk);
        return keyService.VerifySignedHash(data, signature);
    }

    private static IKeyService GetKeyService(JsonWebKey jwk)
    {
        if (jwk.Kty == "EC")
            return new EcKeyService(jwk);
        else if (jwk.Kty == "RSA")
            return new RsaKeyService(jwk);
        
        throw new ApplicationException($"not supported key type {jwk.Kty}");
    }
}