
// https://gunnarpeipman.com/aspnet/aspnet-core-websocket-chart/
// https://www.codetinkerer.com/2018/06/05/aspnet-core-websockets.html
// https://www.limilabs.com/blog/import-certificate-private-public-keys-pem-cer-pfx
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


            // https://github.com/bcgit/bc-csharp/tree/096c4894f62b7f4178ab869ec342e9351bd198dd/crypto/src/crypto/digests
            public static byte[] Keccak(string text)
            {
                byte[] encData = System.Text.Encoding.UTF8.GetBytes(text);
                Org.BouncyCastle.Crypto.Digests.KeccakDigest myHash = new Org.BouncyCastle.Crypto.Digests.KeccakDigest();

                myHash.BlockUpdate(encData, 0, encData.Length);
                byte[] compArr = new byte[myHash.GetDigestSize()];
                myHash.DoFinal(compArr, 0);

                return compArr;
            }


            // https://github.com/bcgit/bc-csharp/tree/096c4894f62b7f4178ab869ec342e9351bd198dd/crypto/src/crypto/digests
            public static byte[] Sha3(string text)
            {
                byte[] encData = System.Text.Encoding.UTF8.GetBytes(text);
                Org.BouncyCastle.Crypto.Digests.Sha3Digest myHash = new Org.BouncyCastle.Crypto.Digests.Sha3Digest();

                myHash.BlockUpdate(encData, 0, encData.Length);
                byte[] compArr = new byte[myHash.GetDigestSize()];
                myHash.DoFinal(compArr, 0);

                return compArr;
            }


            public object GetResult()
            {
                // Org.BouncyCastle.Crypto.Tls.TlsContext
                // https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/tls/Chacha20Poly1305.cs
                // var ccp = new Org.BouncyCastle.Crypto.Tls.Chacha20Poly1305(null);


                Org.BouncyCastle.Asn1.DerObjectIdentifier cc = Org.BouncyCastle.Asn1.X509.X509Name.CountryOfCitizenship;
                Org.BouncyCastle.Asn1.DerObjectIdentifier cr = Org.BouncyCastle.Asn1.X509.X509Name.CountryOfResidence;
                Org.BouncyCastle.Asn1.DerObjectIdentifier coi = Org.BouncyCastle.Asn1.X509.X509Name.OrganizationIdentifier;

                // https://deliciousbrains.com/ssl-certificate-authority-for-local-https-development/
                // https://dzone.com/articles/creating-self-signed-certificate
                // https://stackoverflow.com/questions/10175812/how-to-create-a-self-signed-certificate-with-openssl
                // https://www.akadia.com/services/ssh_test_certificate.html
                // https://coderanch.com/how-to/javadoc/itext-2.1.7/com/lowagie/text/pdf/PdfPKCS7.X509Name.html#CN


                var oc = Org.BouncyCastle.Asn1.X509.X509Name.C; // Country code
                var oST = Org.BouncyCastle.Asn1.X509.X509Name.ST; // State or Province
                var ol = Org.BouncyCastle.Asn1.X509.X509Name.L; // Locality
                var oo = Org.BouncyCastle.Asn1.X509.X509Name.O; // Organization name
                var ou = Org.BouncyCastle.Asn1.X509.X509Name.OU; // Organizational Unit Name 
                var ocn = Org.BouncyCastle.Asn1.X509.X509Name.CN; // Common name
                var oce = Org.BouncyCastle.Asn1.X509.X509Name.E; // email address in Verisign certificates
                var ocee = Org.BouncyCastle.Asn1.X509.X509Name.EmailAddress; // Email address (RSA PKCS#9 extension) - IA5String


                Org.BouncyCastle.Asn1.X509.X509Name a = new Org.BouncyCastle.Asn1.X509.X509Name("ou");


                throw new System.NotImplementedException();
            }
        }

        public Org.BouncyCastle.Crypto.IStreamCalculator CreateCalculator()
        {
            return new MyStreamCalculator(); ;
        }
    }


}
