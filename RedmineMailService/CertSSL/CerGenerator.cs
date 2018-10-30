
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.X509;



// https://github.com/rlipscombe/bouncy-castle-csharp/blob/master/CreateCertificate/Program.cs
// https://github.com/rlipscombe/bouncy-castle-csharp
// http://blog.differentpla.net/blog/2013/03/18/using-bouncy-castle-from-net
// http://blog.differentpla.net/blog/2013/03/18/how-do-i-create-a-self-signed-certificate-using-bouncy-castle
// http://blog.differentpla.net/blog/2013/03/18/how-do-i-convert-a-bouncy-castle-certificate-to-a-net-certificate
// https://code-examples.net/en/q/398779
namespace AnySqlWebAdmin
{
    
    
    public class CerGenerator
    {
        
        static Org.BouncyCastle.Security.SecureRandom secureRandom = new Org.BouncyCastle.Security.SecureRandom();
        
        
        static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(int length)
        {
            Org.BouncyCastle.Crypto.KeyGenerationParameters keygenParam = 
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, length);

            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator keyGenerator = 
                new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateRsaKeyPair 


        static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcKeyPair(string curveName)
        {
            X9ECParameters ecParam = SecNamedCurves.GetByName(curveName);

            return GenerateEcKeyPair(ecParam);
        } // End Function GenerateEcKeyPair 


        static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcKeyPair(X9ECParameters ecParam)
        {
            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters ecDomain = 
                new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(ecParam.Curve, ecParam.G, ecParam.N);

            Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters keygenParam = 
                new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(ecDomain, secureRandom);

            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator keyGenerator = 
                new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();

            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateEcKeyPair 
        

        static X509Certificate GenerateCertificate(
            X509Name issuer, X509Name subject,
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter issuerPrivate,
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter subjectPublic)
        {
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory;
            if (issuerPrivate is Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)
            {

                // var names = Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory.SignatureAlgNames;
                // System.Console.WriteLine(names);
                // string x9 = X9ObjectIdentifiers.ECDsaWithSha256.ToString();
                // System.Console.WriteLine(x9);

                // X9ObjectIdentifiers.ECDsaWithSha512
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                    X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
                    issuerPrivate);
            }
            else
            {
                // PkcsObjectIdentifiers.Sha512WithRsaEncryption
                
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                    PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
                    issuerPrivate);
            }

            var serialNumber = Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(
                Org.BouncyCastle.Math.BigInteger.One, 
                Org.BouncyCastle.Math.BigInteger.ValueOf(System.Int64.MaxValue), secureRandom
            );
            
            
            // X509Name subjectDN = new X509Name("CN=" + commonNameValue);
            
            X509V3CertificateGenerator certGenerator = new X509V3CertificateGenerator();
            certGenerator.SetIssuerDN(issuer);
            certGenerator.SetSubjectDN(subject);
            
            // The certificate needs a serial number. 
            // This is used for revocation, and usually should be an incrementing index 
            // (which makes it easier to revoke a range of certificates).
            // Since we don’t have anywhere to store the incrementing index, we can just use a random number.
            // certGenerator.SetSerialNumber(serialNumber);
            certGenerator.SetSerialNumber(Org.BouncyCastle.Math.BigInteger.ValueOf(1));
            
            certGenerator.SetNotAfter(System.DateTime.UtcNow.AddHours(1));
            certGenerator.SetNotBefore(System.DateTime.UtcNow);
            certGenerator.SetPublicKey(subjectPublic);

            // https://www.programcreek.com/java-api-examples/?class=org.bouncycastle.x509.X509V3CertificateGenerator&method=setSubjectDN
            // certGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Name.C.Id, true, new X509Name("CH"));

            // https://en.wikipedia.org/wiki/Subject_Alternative_Name
            // byte[] subjectAltName = new GeneralNames(new GeneralName(GeneralName.DnsName, "localhost")).GetDerEncoded();
            // certGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName, false, subjectAltName);

            // https://www.programcreek.com/java-api-examples/?api=org.bouncycastle.cert.X509v3CertificateBuilder
            DerSequence subjectAlternativeNames = new DerSequence(new Asn1Encodable[] {
                new GeneralName(GeneralName.DnsName, "localhost"),
                 new GeneralName(GeneralName.DnsName, System.Environment.MachineName),
                new GeneralName(GeneralName.DnsName, "127.0.0.1")
            });

            certGenerator.AddExtension(
                  Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName
                , false
                , subjectAlternativeNames
            );
            
            // https://security.stackexchange.com/questions/169217/certificate-chain-is-broken
            // certGenerator.AddExtension(X509Extensions.BasicConstraints.Id, true, new BasicConstraints(3));


            //Key Usage
            // certGenerator.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment));
            // certGenerator.AddExtension(X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(new[] { KeyPurposeID.IdKPClientAuth, KeyPurposeID.IdKPServerAuth }));


            // certGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Name.C, false, new GeneralName(GeneralName.Rfc822Name, "CH"));

            return certGenerator.Generate(signatureFactory);
        } // End Function GenerateCertificate 


        static bool ValidateSelfSignedCert(X509Certificate cert,
            Org.BouncyCastle.Crypto.ICipherParameters pubKey)
        {
            cert.CheckValidity(System.DateTime.UtcNow);
            byte[] tbsCert = cert.GetTbsCertificate(); // (TBS is short for To Be Signed), See RFC5280 for all the gory details.
            byte[] sig = cert.GetSignature();

            Org.BouncyCastle.Crypto.ISigner signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner(
                cert.SigAlgName
            );
            
            signer.Init(false, pubKey);
            signer.BlockUpdate(tbsCert, 0, tbsCert.Length);
            return signer.VerifySignature(sig);
        } // End Function ValidateSelfSignedCert 


        // https://gist.github.com/Venomed/5337717aadfb61b09e58
        public static void CreateSignatureRequest()
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair caKey25519 = GenerateEcKeyPair(
             Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName("curve25519")
            );

            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory;
            
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey = caKey25519.Public;
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter signingKey = caKey25519.Private;

            // signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory( PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(), issuerPrivate);
            signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
                signingKey);
            
            
            System.Collections.IDictionary attrs = 
                new System.Collections.Generic.Dictionary<DerObjectIdentifier, string>();
            
            // https://codereview.stackexchange.com/questions/84752/net-bouncycastle-csr-and-private-key-generation
            string domainName = "foo.com";
            string companyName = "SkyNet";
            string city = "NeutralZone";
            string state = "ERA";
            string countryIso2Characters = "EA";
            string division = "Skynet Eearth";
            string email = "root@sky.net";
            
            attrs.Add(X509Name.CN, domainName);
            attrs.Add(X509Name.O, companyName);
            attrs.Add(X509Name.L, city);
            attrs.Add(X509Name.ST, state);
            attrs.Add(X509Name.C, countryIso2Characters);

            if (division != null)
            {
                attrs.Add(X509Name.OU, division);
            }

            if (email != null)
            {
                attrs.Add(X509Name.EmailAddress, email);
            }

            X509Name subject = new X509Name(new System.Collections.ArrayList(attrs.Keys), attrs);



            var extensions = new System.Collections.Generic.Dictionary<
                DerObjectIdentifier, X509Extension>()
            {
                {
                    X509Extensions.BasicConstraints, 
                    new X509Extension(true, new DerOctetString(new BasicConstraints(false)))
                    
                },
                {
                    X509Extensions.KeyUsage, 
                    new X509Extension(true, 
                        new DerOctetString(
                            new KeyUsage(KeyUsage.DigitalSignature 
                                     | KeyUsage.KeyEncipherment 
                                     | KeyUsage.DataEncipherment 
                                     | KeyUsage.NonRepudiation)
                        )
                    )
                },
                {
                    X509Extensions.ExtendedKeyUsage, 
                    new X509Extension(false, 
                        new DerOctetString(
                            new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth, KeyPurposeID.IdKPClientAuth)
                        )
                    )
                    
                },
            };
            
            
            // Asn1Set attributes = null;
            Asn1Set attributes = new DerSet(
                new AttributePkcs(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest
                    , new DerSet(new X509Extensions(extensions))
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
            string foo = csrPem.ToString();
            System.Console.WriteLine(foo);
            
            //req.GetDerEncoded();
        }
        
        
        public static void Test()
        {
            X509Name caName = new X509Name("CN=TestCA");
            X509Name eeName = new X509Name("CN=TestEE");
            X509Name eeName25519 = new X509Name("CN=TestEE25519");
            
            
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair caKey25519 = GenerateEcKeyPair("curve25519");
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair caKey25519  = GenerateEcKeyPair(
                Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName("curve25519")
            );
            
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair caKey = GenerateEcKeyPair("secp256r1");
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair eeKey = GenerateRsaKeyPair(2048);


            string publicKey = null;
            // id_rsa.pub
            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(eeKey);
                pemWriter.Writer.Flush();

                publicKey = textWriter.ToString();
            } // End Using textWriter 

            System.Console.WriteLine(publicKey);


            // https://social.msdn.microsoft.com/Forums/vstudio/de-DE/8d49a681-22c6-417f-af3c-8daebd6f10dd/signierung-eines-hashs-mit-ellipticcurve-crypto?forum=visualcsharpde
            // https://stackoverflow.com/questions/22963581/reading-elliptic-curve-private-key-from-file-with-bouncycastle/41947163
            // PKCS8EncodedKeySpec spec = new PKCS8EncodedKeySpec(pkcs8key);

            // The EC PARAMETERS block in your file is an accident of the way openssl ecparam - genkey works by default; 
            // it is not needed or used as part of the actual key and you can omit it by specifying - noout 
            // which is admittedly somewhat unobvious.
            // The actual key structure('hidden' in the base64/ DER data) for EC(DSA / DH) 
            // does contain some parameter info which RSA doesn't but DSA does. 
            RedmineMailService.CertSSL.KeyImportExport.WritePrivatePublic(caKey25519);


            X509Certificate caCert = GenerateCertificate(caName, caName, caKey.Private, caKey.Public);
            X509Certificate eeCert = GenerateCertificate(caName, eeName, caKey.Private, eeKey.Public);
            X509Certificate ee25519Cert = GenerateCertificate(caName, eeName25519, caKey25519.Private, caKey25519.Public);
            
            
            
            bool caOk = ValidateSelfSignedCert(caCert, caKey.Public);
            bool eeOk = ValidateSelfSignedCert(eeCert, caKey.Public);
            bool ee25519 = ValidateSelfSignedCert(eeCert, caKey.Public);


            // System.IO.File.WriteAllBytes("fileName", caCert.Export(X509ContentType.Pkcs12, PfxPassword));
            
            // https://info.ssl.com/how-to-der-vs-crt-vs-cer-vs-pem-certificates-and-how-to-conver-them/
            // The file extensions .CRT and .CER are interchangeable. 
            // If your server requires that you use the .CER file extension, you can change the extension 
            // http://www.networksolutions.com/support/what-is-the-difference-between-a-crt-and-a-cer-file/
            // https://stackoverflow.com/questions/642284/apache-with-ssl-how-to-convert-cer-to-crt-certificates
            // File extensions for cryptographic certificates aren't really as standardized as you'd expect. 
            // Windows by default treats double - clicking a.crt file as a request to import the certificate 
            // So, they're different in that sense, at least, that Windows has some inherent different meaning 
            // for what happens when you double click each type of file.
            
            // One is a "binary" X.509 encoding, and the other is a "text" base64 encoding that usually starts with "-----BEGIN CERTIFICATE-----". 
            // into the Windows Root Certificate store, but treats a.cer file as a request just to view the certificate. 
            // CER is an X.509 certificate in binary form, DER encoded
            // CRT is a binary X.509 certificate, encapsulated in text (base-64) encoding
            // Most systems accept both formats, but if you need to you can convert one to the other via openssl 
            // Certificate file should be PEM-encoded X.509 Certificate file:
            // openssl x509 -inform DER -in certificate.cer -out certificate.pem
            using (System.IO.Stream f = System.IO.File.OpenWrite("ca.cer"))
            {
                byte[] buf = caCert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }
            
            using (System.IO.Stream fs = System.IO.File.OpenWrite("ee.cer"))
            {
                byte[] buf = eeCert.GetEncoded();
                fs.Write(buf, 0, buf.Length);
            } // End Using fs 
            
            using (System.IO.Stream fs = System.IO.File.OpenWrite("ee25519.cer"))
            {
                byte[] buf = ee25519Cert.GetEncoded();
                fs.Write(buf, 0, buf.Length);
            } // End Using fs 
            
            // new System.Text.ASCIIEncoding(false)
            // new System.Text.UTF8Encoding(false)
            using (System.IO.Stream fs = System.IO.File.OpenWrite("ee.crt"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    byte[] buf = eeCert.GetEncoded();
                    string pem = ToPem(buf);

                    sw.Write(pem);
                } // End Using sw 

            } // End Using fs 

            using (System.IO.Stream fs = System.IO.File.OpenWrite("ee25519.crt"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    byte[] buf = ee25519Cert.GetEncoded();
                    string pem = ToPem(buf);

                    sw.Write(pem);
                } // End Using sw 

            } // End Using fs 

            X509Name subject = eeName25519;
            DumpPfx(ee25519Cert, subject, caKey25519);
        } // End Sub Test 


        public static void DumpPfx(
              X509Certificate bouncyCert
            , X509Name subject
            , Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair pair)
        {
            Org.BouncyCastle.Pkcs.Pkcs12Store store = new Org.BouncyCastle.Pkcs.Pkcs12Store();
            Org.BouncyCastle.Pkcs.X509CertificateEntry certificateEntry =
                new Org.BouncyCastle.Pkcs.X509CertificateEntry(bouncyCert);

            store.SetCertificateEntry(subject.ToString(), certificateEntry);

            store.SetKeyEntry(subject.ToString(),
                  new Org.BouncyCastle.Pkcs.AsymmetricKeyEntry(pair.Private)
                , new[] { certificateEntry });

            Org.BouncyCastle.Security.SecureRandom random =
                new Org.BouncyCastle.Security.SecureRandom(
                new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator()
                );

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                string tempPassword = "password";
                store.Save(stream, tempPassword.ToCharArray(), random);
                using (System.Security.Cryptography.X509Certificates.X509Certificate2 cert =
                    new System.Security.Cryptography.X509Certificates.X509Certificate2(
                      stream.ToArray()
                    , tempPassword
                    , System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet
                    | System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable)
                    )
                {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();

                    builder.AppendLine("-----BEGIN CERTIFICATE-----");
                    builder.AppendLine(System.Convert.ToBase64String(
                        cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert)
                        , System.Base64FormattingOptions.InsertLineBreaks)
                    );
                    builder.AppendLine("-----END CERTIFICATE-----");

                    // PFX
                    //builder.ToString().Dump("Self-signed Certificate");
                }
            }
        }
        
        
        
        // Most systems accept both formats, but if you need to you can convert one to the other via openssl 
        // Certificate file should be PEM-encoded X.509 Certificate file:
        // openssl x509 -inform DER -in certificate.cer -out certificate.pem

        // Note: The PEM format is the most common format used for certificates. 
        // Extensions used for PEM certificates are cer, crt, and pem. 
        // They are Base64 encoded ASCII files.The DER format is the binary form of the certificate. 
        // DER formatted certificates do not contain the "BEGIN CERTIFICATE/END CERTIFICATE" statements. 
        // DER formatted certificates most often use the '.der' extension.
        // Note: 
        // https://stackoverflow.com/questions/642284/apache-with-ssl-how-to-convert-cer-to-crt-certificates
        // https://knowledge.digicert.com/solution/SO26449.html
        // https://info.ssl.com/how-to-der-vs-crt-vs-cer-vs-pem-certificates-and-how-to-conver-them/
        public static string ToPem(byte[] buf)
        {
            string cert_begin = "-----BEGIN CERTIFICATE-----\n";
            string end_cert = "-----END CERTIFICATE-----";
            string pem = System.Convert.ToBase64String(buf);

            string pemCert = cert_begin + pem + end_cert;
            return pemCert;
        } // End Function ToPem 
        
        
    } // End Class CerGenerator 
    
    
} // End Namespace AnySqlWebAdmin 
