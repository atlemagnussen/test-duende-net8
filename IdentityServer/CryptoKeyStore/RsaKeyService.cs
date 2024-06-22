using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TestDuende.IdentityServer.CryptoKeyStore;

public class RsaKeyService : IKeyService
{
    JsonWebKey _rsaJwk;
    private RSA _rsaKey;
    
    public RsaKeyService(JsonWebKey rsaJwk)
    {
        _rsaJwk = rsaJwk;
        var parms = ParamsFromJwk(rsaJwk);
        _rsaKey = RSA.Create(parms);
    }

    public string SignJwt(string jwt)
    {
        try
        {
            var encoder = new UTF8Encoding();
            byte[] jwtBytes = encoder.GetBytes(jwt);
            return SignJwt(jwtBytes);
        }
        catch (CryptographicException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private string SignJwt(byte[] jwtBytes)
    {
        var signedBytes = _rsaKey.SignData(jwtBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Base64UrlEncoder.Encode(signedBytes);
    }

    public SecurityKey GetSecurityKeyFromJwk()
    {
        SecurityKey key = new RsaSecurityKey(_rsaKey)
        {
            KeyId = _rsaJwk.Kid
        };
        return key;
    }

    private RSAParameters ParamsFromJwk(JsonWebKey jwk) {
        RSAParameters parms = new RSAParameters
        {
            // PUBLIC KEY PARAMETERS
            Modulus = Base64UrlEncoder.DecodeBytes(jwk.N),
            Exponent = Base64UrlEncoder.DecodeBytes(jwk.E),
        };
        if (jwk.HasPrivateKey) {
            // PRIVATE KEY PARAMETERS (optional)
            parms.D = Base64UrlEncoder.DecodeBytes(jwk.D);
            parms.DP = Base64UrlEncoder.DecodeBytes(jwk.DP);
            parms.DQ = Base64UrlEncoder.DecodeBytes(jwk.DQ);
            parms.P = Base64UrlEncoder.DecodeBytes(jwk.P);
            parms.Q = Base64UrlEncoder.DecodeBytes(jwk.Q);
            parms.InverseQ = Base64UrlEncoder.DecodeBytes(jwk.QI);
        }
        return parms;
    }

    // public static byte[] HashAndSignBytes(byte[] DataToSign, RSAParameters Key)
    // {
    //     try
    //     {
    //         using var rsa = RSA.Create(Key);
    //         return rsa.SignData(DataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    //     }
    //     catch(CryptographicException e)
    //     {
    //         Console.WriteLine(e.Message);
    //         throw;
    //     }
    // }

    public bool VerifySignedHash(string dataBase64, string signatureBase64)
    {
        var encoder = new UTF8Encoding();
        byte[] data = encoder.GetBytes(dataBase64);
        var signature = Base64UrlEncoder.DecodeBytes(signatureBase64);
        return VerifySignedHash(data, signature);
    }
    private bool VerifySignedHash(byte[] data, byte[] signature)
    {
        return _rsaKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}