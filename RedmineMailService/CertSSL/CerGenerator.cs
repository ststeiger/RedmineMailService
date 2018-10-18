
using System.IO;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;


namespace AnySqlWebAdmin
{


    public class CerGenerator
    {
        static SecureRandom secureRandom = new SecureRandom();

        static AsymmetricCipherKeyPair GenerateRsaKeyPair(int length)
        {
            KeyGenerationParameters keygenParam = new KeyGenerationParameters(secureRandom, length);

            RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        }


        static AsymmetricCipherKeyPair GenerateEcKeyPair(string curveName)
        {
            X9ECParameters ecParam = SecNamedCurves.GetByName(curveName);

            return GenerateEcKeyPair(ecParam);
        }


        static AsymmetricCipherKeyPair GenerateEcKeyPair(X9ECParameters ecParam)
        {
            ECDomainParameters ecDomain = new ECDomainParameters(ecParam.Curve, ecParam.G, ecParam.N);
            ECKeyGenerationParameters keygenParam = new ECKeyGenerationParameters(ecDomain, secureRandom);

            ECKeyPairGenerator keyGenerator = new ECKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        }


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
                public Stream Stream => throw new System.NotImplementedException();

                public object GetResult()
                {
                    //Org.BouncyCastle.Crypto.Tls.TlsContext
                    //var ccp = new Org.BouncyCastle.Crypto.Tls.Chacha20Poly1305(null);

                    throw new System.NotImplementedException();
                }
            }

            public IStreamCalculator CreateCalculator()
            {
                return new MyStreamCalculator(); ;
            }
        }


        static X509Certificate GenerateCertificate(
            X509Name issuer, X509Name subject,
            AsymmetricKeyParameter issuerPrivate,
            AsymmetricKeyParameter subjectPublic)
        {
            ISignatureFactory signatureFactory;
            if (issuerPrivate is ECPrivateKeyParameters)
            {


                // PkcsObjectIdentifiers.Sha256WithRsaEncryptio


                // signatureFactory = new Asn1SignatureFactory("Chacha20Poly1305", issuerPrivate);
                // var names = Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory.SignatureAlgNames;
                // System.Console.WriteLine(names);
                // string pkcs = PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString();
                // string x9 = X9ObjectIdentifiers.ECDsaWithSha256.ToString();
                // System.Console.WriteLine(x9);







                // X9ObjectIdentifiers.ECDsaWithSha512
                signatureFactory = new Asn1SignatureFactory(
                    X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
                    issuerPrivate);
            }
            else
            {
                // PkcsObjectIdentifiers.Sha512WithRsaEncryption
                
                signatureFactory = new Asn1SignatureFactory(
                    PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
                    issuerPrivate);
            }

            X509V3CertificateGenerator certGenerator = new X509V3CertificateGenerator();
            certGenerator.SetIssuerDN(issuer);
            certGenerator.SetSubjectDN(subject);
            certGenerator.SetSerialNumber(BigInteger.ValueOf(1));
            certGenerator.SetNotAfter(System.DateTime.UtcNow.AddHours(1));
            certGenerator.SetNotBefore(System.DateTime.UtcNow);
            certGenerator.SetPublicKey(subjectPublic);

            

            return certGenerator.Generate(signatureFactory);
        }

        static bool ValidateSelfSignedCert(X509Certificate cert, ICipherParameters pubKey)
        {
            cert.CheckValidity(System.DateTime.UtcNow);
            byte[] tbsCert = cert.GetTbsCertificate();
            byte[] sig = cert.GetSignature();

            ISigner signer = SignerUtilities.GetSigner(cert.SigAlgName);
            signer.Init(false, pubKey);
            signer.BlockUpdate(tbsCert, 0, tbsCert.Length);
            return signer.VerifySignature(sig);
        }

        public static void Test()
        {
            X509Name caName = new X509Name("CN=TestCA");

            X509Name eeName = new X509Name("CN=TestEE");
            X509Name eeName25519 = new X509Name("CN=TestEE25519");


            // AsymmetricCipherKeyPair caKey25519 = GenerateEcKeyPair("curve25519");
            AsymmetricCipherKeyPair caKey25519  = GenerateEcKeyPair(Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName("curve25519"));
            System.Console.WriteLine(caKey25519);


            AsymmetricCipherKeyPair caKey = GenerateEcKeyPair("secp256r1");
            AsymmetricCipherKeyPair eeKey = GenerateRsaKeyPair(2048);

            X509Certificate caCert = GenerateCertificate(caName, caName, caKey.Private, caKey.Public);
            X509Certificate eeCert = GenerateCertificate(caName, eeName, caKey.Private, eeKey.Public);
            X509Certificate ee25519Cert = GenerateCertificate(caName, eeName25519, caKey25519.Private, caKey25519.Public);

            bool caOk = ValidateSelfSignedCert(caCert, caKey.Public);
            bool eeOk = ValidateSelfSignedCert(eeCert, caKey.Public);
            bool ee25519 = ValidateSelfSignedCert(eeCert, caKey.Public);

            using (System.IO.Stream f = System.IO.File.OpenWrite("ca.cer"))
            {
                byte[] buf = caCert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }

            using (System.IO.Stream f = System.IO.File.OpenWrite("ee.cer"))
            {
                byte[] buf = eeCert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }

            using (System.IO.Stream f = System.IO.File.OpenWrite("ee25519.cer"))
            {
                byte[] buf = ee25519Cert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }
        }
    }


}
