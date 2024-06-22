using Microsoft.IdentityModel.Tokens;

namespace TestDuende.IdentityServer.CryptoKeyStore;

public interface IKeyService
{
    string SignJwt(string jwt);
    SecurityKey GetSecurityKeyFromJwk();
    bool VerifySignedHash(string dataBase64, string signatureBase64);
}