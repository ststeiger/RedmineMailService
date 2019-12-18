
// PemWriter: which one ?
// using Org.BouncyCastle.OpenSsl;
// using Org.BouncyCastle.Utilities.IO.Pem;


// using Org.BouncyCastle.OpenSsl;

// using System.Security.Cryptography.X509Certificates;

using System.Numerics;
using MimeKit.Cryptography;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

namespace RedmineMailService.CertSSL.SSL
{
    
    
    public class PfxConverter
    {

        public static AsymmetricKeyParameter TransformRSAPrivateKey(System.Security.Cryptography.AsymmetricAlgorithm privateKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider prov = privateKey as System.Security.Cryptography.RSACryptoServiceProvider;
            System.Security.Cryptography.RSAParameters parameters = prov.ExportParameters(true);
            
            // Obviously this assumes that the certificate includes a RSA Key but the same result can be achieved for DSA with DSACryptoServiceProvider and DSAParameters
            
            return new RsaPrivateCrtKeyParameters(
                new BigInteger(1,parameters.Modulus),
                new BigInteger(1,parameters.Exponent),
                new BigInteger(1,parameters.D),
                new BigInteger(1,parameters.P),
                new BigInteger(1,parameters.Q),
                new BigInteger(1,parameters.DP),
                new BigInteger(1,parameters.DQ),
                new BigInteger(1,parameters.InverseQ));
        }
        
        
        // https://www.csharpcodi.com/csharp-examples/Org.BouncyCastle.X509.X509Certificate.GetPublicKey()/
        public static bool CheckRequestSignature(
            byte[] serializedSpeechletRequest, string expectedSignature, Org.BouncyCastle.X509.X509Certificate cert) {
 
            byte[] expectedSig = null;
            try {
                expectedSig = System.Convert.FromBase64String(expectedSignature);
            }
            catch (System.FormatException) {
                return false;
            }
 
            var publicKey = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)cert.GetPublicKey();
            var signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner("Sdk.SIGNATURE_ALGORITHM");
            signer.Init(false, publicKey);
            signer.BlockUpdate(serializedSpeechletRequest, 0, serializedSpeechletRequest.Length);            
 
            return signer.VerifySignature(expectedSig);
        }
        

        public static void CreateSignature(System.Security.Cryptography.X509Certificates.X509Certificate2 mycert)
        {
            X509CertificateParser certParser = new X509CertificateParser();
            
            using(var fs = System.IO.File.OpenRead(""))
            {
                certParser.ReadCertificate(fs);
            }


            var xs = new X509CertificateStore(); 
            xs.Import("");
            foreach (X509Certificate thisCert in xs.Certificates)
            {
                thisCert.GetPublicKey();
                
                
                // var signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner(Sdk.SIGNATURE_ALGORITHM);
                
            }

            // https://www.programcreek.com/java-api-examples/?api=org.bouncycastle.x509.X509V3CertificateGenerator
            // https://forums.asp.net/t/2154987.aspx?Create+Self+Signed+Certificate+programatically+uisng+C+
            
            // System.Security.Cryptography.X509Certificates.X509Certificate2.CreateFromCertFile()

            
            // https://overcoder.net/q/429916/bouncycastle-privatekey-to-x509%D1%81%D0%B5%D1%80%D1%82%D0%B8%D1%84%D0%B8%D0%BA%D0%B0%D1%822-privatekey
            // https://www.csharpcodi.com/csharp-examples/Org.BouncyCastle.X509.X509Certificate.GetPublicKey()/
            
            AsymmetricKeyParameter Akp = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(mycert.PrivateKey).Private;

            
            // if(mycert.HasPrivateKey)
            AsymmetricKeyParameter bouncyCastlePrivateKey = TransformRSAPrivateKey(mycert.PrivateKey);
            
            
            
            
            
            X509Certificate bouncyCertificate = certParser.ReadCertificate(mycert.GetRawCertData());
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter pubKey = bouncyCertificate.GetPublicKey();
            
            var algorithm = Org.BouncyCastle.Security.DigestUtilities.GetDigest(bouncyCertificate.SigAlgOid);
            // var signature = new X509Certificate2Signature(mycert, algorithm);
            // https://github.com/kusl/itextsharp/blob/master/tags/iTextSharp_5_4_5/src/core/iTextSharp/text/pdf/security/X509Certificate2Signature.cs
            // Sign

            // PemReader pem = new PemReader();
            // pem.ReadPemObject().Headers
            // RSACryptoServiceProvider rsa = pem.ReadPrivateKeyFromFile("PrivateKey.pem");
        }


        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/80ccc76f-bf98-4cda-9583-f651013b24a5/extract-private-key-as-string-from-pfx-file?forum=csharpgeneral
        // One of my collegues actually found the solution and I thought I'd share it.
        // Extract Private Key as String from PFX File
        public static void GetPrivateKey(string pfxLocation, string password)
        {
            // Windows's PFX files are just renamed PKCS#12 files,
            
            // Load your certificate from file
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = 
                new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxLocation, password
                    , System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable 
                      | System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet);

            
            // Private Key
            if (certificate.HasPrivateKey)
            {
                throw new System.IO.InvalidDataException("no private key in pfx file.");
            }

            System.Security.Cryptography.RSACryptoServiceProvider rsa = (System.Security.Cryptography.RSACryptoServiceProvider)certificate.PrivateKey;
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            System.IO.TextWriter streamWriter = new System.IO.StreamWriter(memoryStream);
            
            
            Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(streamWriter);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = Org.BouncyCastle.Security.DotNetUtilities.GetRsaKeyPair(rsa);
            pemWriter.WriteObject(keyPair.Private);
            streamWriter.Flush();
            string output = System.Text.Encoding.ASCII.GetString(memoryStream.GetBuffer()).Trim();
            int index_of_footer = output.IndexOf("-----END RSA PRIVATE KEY-----");
            memoryStream.Close();
            streamWriter.Close();
            string PrivKey = output.Substring(0, index_of_footer + 29);
        }
        
        
        public static void GetPublicKey(string pfxLocation, string password)
        {
            // I'm trying to mimic OpenSSL's capability to extract the Private Key from a PFX Bundle into it's own file.
            // I can get the Public Key OK with the following code:
            
            // Load your certificate from file
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = 
                new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxLocation, password
                    , System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable 
                      | System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet);
            // Public Key;
            System.Text.StringBuilder publicBuilder = new System.Text.StringBuilder();
            publicBuilder.AppendLine("-----BEGIN CERTIFICATE-----");
            publicBuilder.AppendLine(System.Convert.ToBase64String(certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert)
                , System.Base64FormattingOptions.InsertLineBreaks));
            
            publicBuilder.AppendLine("-----END CERTIFICATE-----");
            string foo  = publicBuilder.ToString();
        }

        
    }
}