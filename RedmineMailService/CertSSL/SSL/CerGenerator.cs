
// using Org.BouncyCastle.Asn1;
// using Org.BouncyCastle.Asn1.Cms;
// using Org.BouncyCastle.Asn1.Pkcs;
// using Org.BouncyCastle.Asn1.Sec;
// using Org.BouncyCastle.Asn1.X509;
// using Org.BouncyCastle.Asn1.X9;
// using Org.BouncyCastle.Pkcs;
// using Org.BouncyCastle.X509;


// http://blog.differentpla.net/blog/2013/03/18/how-do-i-create-a-self-signed-certificate-using-bouncy-castle
// https://github.com/rlipscombe/bouncy-castle-csharp/blob/master/CreateCertificate/Program.cs
// https://github.com/rlipscombe/bouncy-castle-csharp
// http://blog.differentpla.net/blog/2013/03/18/using-bouncy-castle-from-net
// http://blog.differentpla.net/blog/2013/03/18/how-do-i-create-a-self-signed-certificate-using-bouncy-castle
// http://blog.differentpla.net/blog/2013/03/18/how-do-i-convert-a-bouncy-castle-certificate-to-a-net-certificate
// https://code-examples.net/en/q/398779


using RedmineMailService.CertSSL;


namespace AnySqlWebAdmin
{


    public class CerGenerator
    {
        
        
        // https://stackoverflow.com/questions/19270507/correct-way-to-use-random-in-multithread-application
        // static Org.BouncyCastle.Security.SecureRandom s_secureRandom = new Org.BouncyCastle.Security.SecureRandom();
        private static readonly System.Threading.ThreadLocal<Org.BouncyCastle.Security.SecureRandom> s_secureRandom =
            new System.Threading.ThreadLocal<Org.BouncyCastle.Security.SecureRandom>(
                () => new Org.BouncyCastle.Security.SecureRandom(NonBackdooredPrng.Create()));
        
        
        public static Org.BouncyCastle.Crypto.ISignatureFactory CreateSignatureFactory(
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey)
        {
            // https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/operators/Asn1Signature.cs
            // https://github.com/kerryjiang/BouncyCastle.Crypto/blob/master/Crypto/x509/X509Utilities.cs
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = null;
            // Org.BouncyCastle.X509.X509Utilities.GetAlgorithmOid("algorithm");
            
            // new Asn1SignatureFactory("SHA512WITHRSA", issuerKeyPair.Private, rand
            // erObjectIdentifier sigOid = X509Utilities.GetAlgorithmOid(algorithm);
            // this.algID = X509Utilities.GetSigAlgID(sigOid, algorithm);
            
            if (privateKey is Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)
            {
#if false 
                // Values from Org.BouncyCastle.Crypto.Operators.X509Utilities // in Asn1Signature.cs 
                // GOST3411WITHECGOST3410-2001
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                      Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001.ToString()
                    , privateKey
                );
                
                return signatureFactory;
#endif
                
                // Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                      Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha256.ToString()
                      //Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512.ToString()
                    , privateKey
                );
            }
            else if (privateKey is Org.BouncyCastle.Crypto.Parameters.Gost3410KeyParameters)
            {
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                       Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94.ToString()
                     , privateKey
                 );
            }
            else if (privateKey is Org.BouncyCastle.Crypto.Parameters.DsaPrivateKeyParameters)
            {
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                     // Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.DsaWithSha256.ToString()
                     Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.DsaWithSha512.ToString()
                    , privateKey
                );
            }
            else if (privateKey is Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)
            {
                // Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha512WithRsaEncryption
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                     //Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString()
                     Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha512WithRsaEncryption.ToString()
                    , privateKey
                );
            }
            
            return signatureFactory;
        } // End Function CreateSignatureFactory 
        
        
        public static void AddExtensions(Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator
            , CertificateInfo certificateInfo)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, Org.BouncyCastle.Asn1.Asn1Encodable> kvp
                     in certificateInfo.CriticalExtensions)
            {
                certificateGenerator.AddExtension(kvp.Key, true, kvp.Value);
            } // Next kvp 
            
            foreach (System.Collections.Generic.KeyValuePair<string, Org.BouncyCastle.Asn1.Asn1Encodable> kvp 
                in certificateInfo.NonCriticalExtensions)
            {
                certificateGenerator.AddExtension(kvp.Key, false, kvp.Value);
            } // Next kvp 
            
        } // End Sub AddExtensions 
        
        
        public static Org.BouncyCastle.X509.X509Certificate GenerateSslCertificate(
              CertificateInfo certificateInfo
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            , Org.BouncyCastle.X509.X509Certificate rootCertificate
        )
        {
            // The Certificate Generator
            Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator = 
                new Org.BouncyCastle.X509.X509V3CertificateGenerator();
            
            certificateGenerator.SetSubjectDN(certificateInfo.Subject);
            certificateGenerator.SetIssuerDN(rootCertificate.IssuerDN);
            
            
            Org.BouncyCastle.Math.BigInteger serialNumber =
                Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(
                Org.BouncyCastle.Math.BigInteger.One,
                Org.BouncyCastle.Math.BigInteger.ValueOf(System.Int64.MaxValue), secureRandom
            );
            
            
            certificateGenerator.SetSerialNumber(Org.BouncyCastle.Math.BigInteger.ValueOf(1));
            
            
            certificateGenerator.SetNotBefore(certificateInfo.ValidFrom);
            certificateGenerator.SetNotAfter(certificateInfo.ValidTo);
            
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey = 
                KeyImportExport.ReadPublicKey(certificateInfo.SubjectKeyPair.PublicKey);
            
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = 
                KeyImportExport.ReadPrivateKey(certificateInfo.IssuerKeyPair.PrivateKey);
            
            
            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName.Id
                , false
                , certificateInfo.SubjectAlternativeNames
            );
            
            certificateGenerator.SetPublicKey(publicKey);
            
            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id, false,
                new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPServerAuth)
            );
            
            
            // rootCertificate.GetPublicKey():
            // rootCertificate.GetEncoded()
            // System.Security.Cryptography.X509Certificates.X509Certificate2 srp = 
            //    new System.Security.Cryptography.X509Certificates.X509Certificate2(rootCertificate.GetEncoded());
            
            // srp.PrivateKey
            // srp.HasPrivateKey
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter Akp = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(srp.PrivateKey).Private;
            
            // Org.BouncyCastle.Crypto.IDigest algorithm = Org.BouncyCastle.Security.DigestUtilities.GetDigest(rootCertificate.SigAlgOid);
            // var signature = new X509Certificate2Signature(cert, algorithm);
            
            // rootCertificate.SigAlgOid
            // rootCertificate.GetSignature();
            // srp.GetRawCertData()
            
            // X509CertificateParser certParser = new X509CertificateParser();
            // X509Certificate privateCertBouncy = certParser.ReadCertificate(mycert.GetRawCertData());
            // AsymmetricKeyParameter pubKey = privateCertBouncy.GetPublicKey();
            
            
            AddExtensions(certificateGenerator, certificateInfo);
            
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = CreateSignatureFactory(privateKey);
            return certificateGenerator.Generate(signatureFactory);
        } // End Function GenerateSslCertificate 
        
        
        public static Org.BouncyCastle.X509.X509Certificate GenerateRootCertificate(
              CertificateInfo certificateInfo
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            // The Certificate Generator
            Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator = 
                new Org.BouncyCastle.X509.X509V3CertificateGenerator();
            
            Org.BouncyCastle.Asn1.X509.X509Name subjectDn = certificateInfo.Subject;
            Org.BouncyCastle.Asn1.X509.X509Name issuerDn = certificateInfo.Subject;
            
            certificateGenerator.SetSubjectDN(issuerDn);
            certificateGenerator.SetIssuerDN(issuerDn);
            
            
            certificateGenerator.SetNotBefore(certificateInfo.ValidFrom);
            certificateGenerator.SetNotAfter(certificateInfo.ValidTo);
            
            
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey = 
                KeyImportExport.ReadPublicKey(certificateInfo.SubjectKeyPair.PublicKey);
            
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = 
                KeyImportExport.ReadPrivateKey(certificateInfo.SubjectKeyPair.PrivateKey);
            
            
            AddExtensions(certificateGenerator, certificateInfo);
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = CreateSignatureFactory(privateKey);
            
            
            certificateGenerator.SetPublicKey(publicKey);
            
            
            // Serial Number
            Org.BouncyCastle.Math.BigInteger serialNumber =
                Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(
                      Org.BouncyCastle.Math.BigInteger.One
                    , Org.BouncyCastle.Math.BigInteger.ValueOf(long.MaxValue)
                    , secureRandom
            );
            
            certificateGenerator.SetSerialNumber(serialNumber);
            
            
            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.KeyUsage, true
                , new Org.BouncyCastle.Asn1.X509.KeyUsage(
                      Org.BouncyCastle.Asn1.X509.KeyUsage.DigitalSignature
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyEncipherment
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.DataEncipherment
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyCertSign
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyAgreement
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.NonRepudiation
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.CrlSign
                )
            );
            
            
            // Set certificate intended purposes to only Server Authentication
            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id
                , true 
                , new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPServerAuth)
            );
            
            // Only if we generate a root-Certificate 
            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.BasicConstraints.Id
                , true
                , new Org.BouncyCastle.Asn1.X509.BasicConstraints(true)
            );
            
            return certificateGenerator.Generate(signatureFactory);
        } // End Function GenerateRootCertificate 
        
        
        // SHA512WITHRSA
        // SHA512WITHRSAANDMGF1
        // RIPEMD256WITHRSA
        // SHA512WITHDSA
        // SHA512WITHECDSA
        // GOST3411WITHECGOST3410
        private static Org.BouncyCastle.X509.X509Certificate GenerateCertificate(
            Org.BouncyCastle.Asn1.X509.X509Name issuer,
            Org.BouncyCastle.Asn1.X509.X509Name subject,
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter issuerPrivate,
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter subjectPublic,
            Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory;
            if (issuerPrivate is Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)
            {
                // System.Collections.IEnumerable names = Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory.SignatureAlgNames;
                // System.Console.WriteLine(names);
                // string x9 = Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha256.ToString();
                // System.Console.WriteLine(x9);
                
                // Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                    Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
                    issuerPrivate);
            }
            else
            {
                // Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha512WithRsaEncryption
                
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                    Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
                    issuerPrivate);
            }
            
            Org.BouncyCastle.Math.BigInteger serialNumber = 
                Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(
                Org.BouncyCastle.Math.BigInteger.One, 
                Org.BouncyCastle.Math.BigInteger.ValueOf(System.Int64.MaxValue), secureRandom
            );
            
            
            // Org.BouncyCastle.Asn1.X509.X509Name subjectDN = new Org.BouncyCastle.Asn1.X509.X509Name("CN=" + commonNameValue);
            
            Org.BouncyCastle.X509.X509V3CertificateGenerator certGenerator = 
                new Org.BouncyCastle.X509.X509V3CertificateGenerator();
            
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
            
            /*
            // https://www.programcreek.com/java-api-examples/?api=org.bouncycastle.cert.X509v3CertificateBuilder
            Org.BouncyCastle.Asn1.DerSequence subjectAlternativeNames = 
                new Org.BouncyCastle.Asn1.DerSequence(
                    new Org.BouncyCastle.Asn1.Asn1Encodable[] {
                        new Org.BouncyCastle.Asn1.X509.GeneralName(Org.BouncyCastle.Asn1.X509.GeneralName.DnsName, "localhost"),
                        new Org.BouncyCastle.Asn1.X509.GeneralName(Org.BouncyCastle.Asn1.X509.GeneralName.DnsName, System.Environment.MachineName),
                        new Org.BouncyCastle.Asn1.X509.GeneralName(Org.BouncyCastle.Asn1.X509.GeneralName.DnsName, "127.0.0.1")
            });
            
            
            certGenerator.AddExtension(
                  Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName
                , false
                , subjectAlternativeNames
            );
            */

            // https://security.stackexchange.com/questions/169217/certificate-chain-is-broken
            // certGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.BasicConstraints.Id, true, new Org.BouncyCastle.Asn1.X509.BasicConstraints(3));


            //Key Usage
            certGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.KeyUsage, true
                , new Org.BouncyCastle.Asn1.X509.KeyUsage(
                      Org.BouncyCastle.Asn1.X509.KeyUsage.DigitalSignature
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyEncipherment
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.DataEncipherment
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyCertSign
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyAgreement
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.NonRepudiation
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.CrlSign
                    )
            );

            certGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id, true 
                , new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(
                    new[] { Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPClientAuth
                , Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPServerAuth
                })
            );

            return certGenerator.Generate(signatureFactory);
        } // End Function GenerateCertificate 
        
        
        static bool ValidateSelfSignedCert(
            Org.BouncyCastle.X509.X509Certificate cert,
            Org.BouncyCastle.Crypto.ICipherParameters pubKey
            )
        {
            cert.CheckValidity(System.DateTime.UtcNow);
            byte[] tbsCert = cert.GetTbsCertificate(); // (TBS is short for To Be Signed), see RFC5280 for all the gory details.
            byte[] sig = cert.GetSignature();

            Org.BouncyCastle.Crypto.ISigner signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner(
                cert.SigAlgName
            );
            
            signer.Init(false, pubKey);
            signer.BlockUpdate(tbsCert, 0, tbsCert.Length);
            return signer.VerifySignature(sig);
        } // End Function ValidateSelfSignedCert 




        public static void Test2()
        {
            Org.BouncyCastle.X509.X509Certificate caRoot = null;
            Org.BouncyCastle.X509.X509Certificate caSsl = null;
            CertificateInfo caCertInfo = null;

            { 
                string countryIso2Characters = "EA";
                string stateOrProvince = "ERA";
                string localityOrCity = "NeutralZone";
                string companyName = "Skynet mbH";
                string division = "Skynet Earth Inc.";
                string domainName = "sky.net";
                string email = "root@sky.net";



                caCertInfo = new CertificateInfo(
                      countryIso2Characters, stateOrProvince
                    , localityOrCity, companyName
                    , division, domainName, email
                    , System.DateTime.UtcNow
                    , System.DateTime.UtcNow.AddYears(5)
                    );

            

                Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp1 = KeyGenerator.GenerateEcKeyPair("curve25519", s_secureRandom.Value);
                // kp1 = KeyGenerator.GenerateGhostKeyPair(4096, s_secureRandom.Value);

                caCertInfo.SubjectKeyPair = KeyImportExport.GetPemKeyPair(kp1);
                caCertInfo.IssuerKeyPair = KeyImportExport.GetPemKeyPair(kp1);

                caRoot = GenerateRootCertificate(caCertInfo, s_secureRandom.Value);

                PfxGenerator.CreatePfxFile(@"ca.pfx", caRoot, kp1.Private, null, "SkyNet");
            }


            {
                string countryIso2Characters = "GA";
                string stateOrProvince = "Aremorica";
                string localityOrCity = "Erquy, Bretagne";
                string companyName = "Coopérative ménhir Obelix Gmbh & Co. KGaA";
                string division = "NT (Neanderthal Technology)";
                string domainName = "localhost";
                string email = "webmaster@localhost";
                
                
                CertificateInfo ci = new CertificateInfo(
                      countryIso2Characters, stateOrProvince
                    , localityOrCity, companyName
                    , division, domainName, email
                    , System.DateTime.UtcNow
                    , System.DateTime.UtcNow.AddYears(5)
                );
                
                ci.AddAlternativeNames("localhost", System.Environment.MachineName, "127.0.0.1");
                
                Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp1 = KeyGenerator.GenerateEcKeyPair("curve25519", s_secureRandom.Value);
                ci.SubjectKeyPair = KeyImportExport.GetPemKeyPair(kp1);
                ci.IssuerKeyPair = caCertInfo.SubjectKeyPair;
                
                caSsl = GenerateSslCertificate(ci, s_secureRandom.Value, caRoot);
                
                // Just to clarify, an X.509 certificate does not contain the private key
                //  The whole point of using certificates is to send them more or less openly, 
                // without sending the private key, which must be kept secret.
                // An X509Certificate2 object may have a private key associated with it (via its PrivateKey property), 
                // but that's only a convenience as part of the design of this class.
                // var cc = new System.Security.Cryptography.X509Certificates.X509Certificate2(caRoot.GetEncoded());
                // System.Console.WriteLine(cc.PublicKey);
                // System.Console.WriteLine(cc.PrivateKey);
                
                // bool val = ValidateSelfSignedCert(caSsl, caRoot.GetPublicKey());
                // System.Console.WriteLine(val);
                
                PfxGenerator.CreatePfxFile(@"obelix.pfx", caSsl, kp1.Private, "", "Obelix");
            }
            
            WriteCerAndCrt(@"ca", caRoot);
            WriteCerAndCrt(@"obelix", caSsl);
        } // End Sub Test2 
        
        
        public static void WriteCerAndCrt(
             string fileName
            ,Org.BouncyCastle.X509.X509Certificate certificate
            )
        {

            using (System.IO.Stream fs = System.IO.File.OpenWrite( fileName + ".cer"))
            {
                byte[] buf = certificate.GetEncoded();
                fs.Write(buf, 0, buf.Length);
            } // End Using fs 

            // new System.Text.ASCIIEncoding(false)
            // new System.Text.UTF8Encoding(false)
            using (System.IO.Stream fs = System.IO.File.OpenWrite(fileName + ".crt"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    byte[] buf = certificate.GetEncoded();
                    string pem = ToPem(buf);

                    sw.Write(pem);
                } // End Using sw 

            } // End Using fs 
        } // End Sub WriteCerAndCrt 
        
        
        public static void Test()
        {
            Org.BouncyCastle.Asn1.X509.X509Name caName = new Org.BouncyCastle.Asn1.X509.X509Name("CN=TestCA");
            Org.BouncyCastle.Asn1.X509.X509Name eeName = new Org.BouncyCastle.Asn1.X509.X509Name("CN=TestEE");
            Org.BouncyCastle.Asn1.X509.X509Name eeName25519 = new Org.BouncyCastle.Asn1.X509.X509Name("CN=TestEE25519");

            string countryIso2Characters = "EA";
            string stateOrProvince = "ERA";
            string localityOrCity = "NeutralZone";
            string companyName = "Skynet Earth Inc.";
            string division = "Skynet mbH";
            string domainName = "sky.net";
            string email = "root@sky.net";


            Org.BouncyCastle.Asn1.X509.X509Name subj = CertificateInfo.CreateSubject(
                  countryIso2Characters, stateOrProvince
                , localityOrCity, companyName
                , division, domainName, email);

            System.Console.WriteLine(subj);

            
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair caKey25519 = KeyGenerator.GenerateEcKeyPair("curve25519", s_secureRandom.Value);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair caKey = KeyGenerator.GenerateEcKeyPair("secp256r1", s_secureRandom.Value);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair eeKey = KeyGenerator.GenerateRsaKeyPair(2048, s_secureRandom.Value);


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
            PrivatePublicPemKeyPair keyPair = KeyImportExport.GetPemKeyPair(caKey25519);
            
            // PrivatePublicPemKeyPair keyPair = PrivatePublicPemKeyPair.ImportFrom("", "");
            
            
            Org.BouncyCastle.X509.X509Certificate caCert = GenerateCertificate(caName, caName, caKey.Private, caKey.Public, s_secureRandom.Value);
            Org.BouncyCastle.X509.X509Certificate eeCert = GenerateCertificate(caName, eeName, caKey.Private, eeKey.Public, s_secureRandom.Value);
            Org.BouncyCastle.X509.X509Certificate ee25519Cert = GenerateCertificate(caName, eeName25519, caKey25519.Private, caKey25519.Public, s_secureRandom.Value);
            
            
            bool caOk = ValidateSelfSignedCert(caCert, caKey.Public);
            bool eeOk = ValidateSelfSignedCert(eeCert, caKey.Public);
            bool ee25519 = ValidateSelfSignedCert(eeCert, caKey.Public);
            
            PfxGenerator.CreatePfxFile("example.pfx", caCert, caKey.Private, null, "mykey");
            
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
            
            Org.BouncyCastle.Asn1.X509.X509Name subject = eeName25519;
        } // End Sub Test 
        
        
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
            string end_cert = "\n-----END CERTIFICATE-----";
            string pem = System.Convert.ToBase64String(buf);
            
            string pemCert = cert_begin + pem + end_cert;
            return pemCert;
        } // End Function ToPem 
        
        
    } // End Class CerGenerator 
    
    
} // End Namespace AnySqlWebAdmin 
