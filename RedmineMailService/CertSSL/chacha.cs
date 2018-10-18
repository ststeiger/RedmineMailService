
namespace RedmineMailService.CertSSL
{

    // var ccp = new Org.BouncyCastle.Crypto.Tls.Chacha20Poly1305(null);


    // The connection to this site is encrypted and authenticated using TLS 1.2 (a strong protocol), 
    // ECDHE_RSA with X25519(a strong key exchange), // This is SSL-Certificate
    // and AES_128_GCM(a strong cipher). // this is server-setting

    // https://www.openbsd.org/
    // The connection to this site is encrypted and authenticated using TLS 1.2 (a strong protocol), 
    // ECDHE_RSA with X25519(a strong key exchange), // This is SSL-Certificate
    // and CHACHA20_POLY1305(a strong cipher). // this is server-setting

    // https://raymii.org/s/tutorials/Strong_SSL_Security_On_nginx.html
    // https://github.com/openssl/openssl/blob/master/include/openssl/ssl.h
    // BouncyCastle\src\crypto\ec\CustomNamedCurves.cs
    // BouncyCastle\src\asn1\sec\SECNamedCurves.cs
    // BouncyCastle\src\crypto\operators\Asn1Signature.cs
    public class ChaChaFactory
        : Org.BouncyCastle.Crypto.ISignatureFactory
    {
        public object AlgorithmDetails => throw new System.NotImplementedException();

        public class MyStreamCalculator
            : Org.BouncyCastle.Crypto.IStreamCalculator
        {
            public System.IO.Stream Stream => throw new System.NotImplementedException();

            public object GetResult()
            {
                //Org.BouncyCastle.Crypto.Tls.TlsContext
                //var ccp = new Org.BouncyCastle.Crypto.Tls.Chacha20Poly1305(null);

                throw new System.NotImplementedException();
            }
        }

        public Org.BouncyCastle.Crypto.IStreamCalculator CreateCalculator()
        {
            return new MyStreamCalculator(); ;
        }
    }


}
