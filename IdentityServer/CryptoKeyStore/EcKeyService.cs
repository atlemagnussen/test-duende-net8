using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TestDuende.IdentityServer.CryptoKeyStore;

public class EcKeyService : IKeyService
{
    private readonly JsonWebKey _ecJwk;
    private readonly ECDsa _ecKey;
    public EcKeyService(JsonWebKey ecJwk)
    {
        _ecJwk = ecJwk;
        var parms = ParamsFromJwk(ecJwk);
        _ecKey = ECDsa.Create(parms);
    }
    public string SignJwt(string jwt)
    {
        var encoder = new UTF8Encoding();
        byte[] jwtBytes = encoder.GetBytes(jwt);
        return SignJwt(jwtBytes);
    }

    private string SignJwt(byte[] jwtBytes)
    {
        var signedBytes = _ecKey.SignData(jwtBytes, HashAlgorithmName.SHA256);
        return Base64UrlEncoder.Encode(signedBytes);
    }

    private ECParameters ParamsFromJwk(JsonWebKey jwk)
    {
        var curve = jwk.Crv switch
        {
            "P-256" => ECCurve.NamedCurves.nistP256,
            "P-384" => ECCurve.NamedCurves.nistP384,
            "P-521" => ECCurve.NamedCurves.nistP521,
            _ => throw new NotSupportedException()
        };

        var parms = new ECParameters
        {
            Curve = curve,
            Q = new ECPoint
            {
                X = Base64UrlEncoder.DecodeBytes(jwk.X),
                Y = Base64UrlEncoder.DecodeBytes(jwk.Y)
            }
        };
        if (jwk.HasPrivateKey)
            parms.D = Base64UrlEncoder.DecodeBytes(jwk.D);
        
        return parms;
    }

    public SecurityKey GetSecurityKeyFromJwk()
    {
        SecurityKey key = new ECDsaSecurityKey(_ecKey)
        {
            KeyId = _ecJwk.Kid,
        };
        return key;
    }

    public bool VerifySignedHash(string dataBase64, string signatureBase64)
    {
        var encoder = new UTF8Encoding();
        byte[] data = encoder.GetBytes(dataBase64);
        var signature = Base64UrlEncoder.DecodeBytes(signatureBase64);
        return VerifySignedHash(data, signature);
    }
    private bool VerifySignedHash(byte[] data, byte[] signature)
    {
        return _ecKey.VerifyData(data, signature, HashAlgorithmName.SHA256);
    }
}