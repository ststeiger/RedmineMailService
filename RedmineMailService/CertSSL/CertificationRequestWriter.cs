
namespace RedmineMailService.CertSSL
{


    class CertificationRequestWriter
    {


        // https://gist.github.com/Venomed/5337717aadfb61b09e58
        // https://www.powershellgallery.com/packages/Posh-ACME/2.2.0/Content/Private%5CNew-Csr.ps1
        public static string CreateSignatureRequest(
              Org.BouncyCastle.Asn1.X509.X509Name subject
            , Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair caKey25519
            , Org.BouncyCastle.Security.SecureRandom secureRandom)
        {
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory;

            Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey = caKey25519.Public;
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter signingKey = caKey25519.Private;


            if (signingKey is Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)
            {
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                    Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
                    signingKey);
            }
            else
            {
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                    Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
                    signingKey);
            }

            System.Collections.Generic.Dictionary<Org.BouncyCastle.Asn1.DerObjectIdentifier, Org.BouncyCastle.Asn1.X509.X509Extension> extensions =
                new System.Collections.Generic.Dictionary<Org.BouncyCastle.Asn1.DerObjectIdentifier, Org.BouncyCastle.Asn1.X509.X509Extension>()
            {
                {
                          Org.BouncyCastle.Asn1.X509.X509Extensions.BasicConstraints,
                          new Org.BouncyCastle.Asn1.X509.X509Extension(
                              true
                            , new Org.BouncyCastle.Asn1.DerOctetString( new Org.BouncyCastle.Asn1.X509.BasicConstraints(false) )
                          )

                },
                {
                    Org.BouncyCastle.Asn1.X509.X509Extensions.KeyUsage,
                    new Org.BouncyCastle.Asn1.X509.X509Extension(true,
                        new Org.BouncyCastle.Asn1.DerOctetString(
                            new Org.BouncyCastle.Asn1.X509.KeyUsage(
                                       Org.BouncyCastle.Asn1.X509.KeyUsage.DigitalSignature
                                     | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyEncipherment
                                     | Org.BouncyCastle.Asn1.X509.KeyUsage.DataEncipherment
                                     | Org.BouncyCastle.Asn1.X509.KeyUsage.NonRepudiation
                            )
                        )
                    )
                },
                {
                    Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage,
                    new Org.BouncyCastle.Asn1.X509.X509Extension(false,
                        new Org.BouncyCastle.Asn1.DerOctetString(
                            new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(
                                Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPServerAuth,
                                Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPClientAuth
                            )
                        )
                    )

                },
            };


            // Asn1Set attributes = null;
            Org.BouncyCastle.Asn1.Asn1Set attributes =
                new Org.BouncyCastle.Asn1.DerSet(
                    new Org.BouncyCastle.Asn1.Pkcs.AttributePkcs(
                          Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtExtensionRequest
                        , new Org.BouncyCastle.Asn1.DerSet(new Org.BouncyCastle.Asn1.X509.X509Extensions(extensions))
                    )
            );


            Org.BouncyCastle.Pkcs.Pkcs10CertificationRequest csr =
                new Org.BouncyCastle.Pkcs.Pkcs10CertificationRequest(
                    signatureFactory,
                    subject,
                    publicKey,
                    attributes,
                    signingKey
            );


            
            System.Text.StringBuilder csrPem = new System.Text.StringBuilder();
            
            Org.BouncyCastle.OpenSsl.PemWriter csrPemWriter = 
                new Org.BouncyCastle.OpenSsl.PemWriter(new System.IO.StringWriter(csrPem));

            csrPemWriter.WriteObject(csr);
            csrPemWriter.Writer.Flush();
            string signingRequest = csrPem.ToString();
            csrPem.Clear();
            csrPem = null;

            
            // System.IO.File.WriteAllText("request.csr", signingRequest, System.Text.Encoding.ASCII);

            CertificationRequestReader.ReadCertificationRequest(signingRequest);

            // csr.GetDerEncoded();
            System.Console.WriteLine(signingRequest);
            return signingRequest; 
        } // End Sub CreateSignatureRequest 


    } // End Class CertificationRequestWriter 


} // End Namespace RedmineMailService.CertSSL 
