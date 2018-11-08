
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;


using Titanium.Web.Proxy.Shared;


using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Threading;


namespace Titanium.Web.Proxy.Network.Certificate
{
    
    
    /// <summary>
    ///     Implements certificate generation operations.
    /// </summary>
    internal class BCCertificateMaker 
        : ICertificateMaker
    {
        private const int certificateValidDays = 1825;
        private const int certificateGraceDays = 366;
        
        // The FriendlyName value cannot be set on Unix.
        // Set this flag to true when exception detected to avoid further exceptions
        private static bool doNotSetFriendlyName;
        
        private readonly ExceptionHandler exceptionFunc;
        
        
        
        internal BCCertificateMaker(ExceptionHandler exceptionFunc)
        {
            this.exceptionFunc = exceptionFunc;
        }
        
        
        /// <summary>
        ///     Makes the certificate.
        /// </summary>
        /// <param name="sSubjectCn">The s subject cn.</param>
        /// <param name="isRoot">if set to <c>true</c> [is root].</param>
        /// <param name="signingCert">The signing cert.</param>
        /// <returns>X509Certificate2 instance.</returns>
        public X509Certificate2 MakeCertificate(string sSubjectCn, bool isRoot, X509Certificate2 signingCert = null)
        {
            return makeCertificateInternal(sSubjectCn, isRoot, true, signingCert);
        }
        
        
        /// <summary>
        ///     Generates the certificate.
        /// </summary>
        /// <param name="subjectName">Name of the subject.</param>
        /// <param name="issuerName">Name of the issuer.</param>
        /// <param name="validFrom">The valid from.</param>
        /// <param name="validTo">The valid to.</param>
        /// <param name="keyStrength">The key strength.</param>
        /// <param name="signatureAlgorithm">The signature algorithm.</param>
        /// <param name="issuerPrivateKey">The issuer private key.</param>
        /// <param name="hostName">The host name</param>
        /// <returns>X509Certificate2 instance.</returns>
        /// <exception cref="PemException">Malformed sequence in RSA private key</exception>
        private static X509Certificate2 generateCertificate(string hostName,
            string subjectName,
            string issuerName, System.DateTime validFrom,
            System.DateTime validTo, int keyStrength = 2048,
            string signatureAlgorithm = "SHA256WithRSA",
            AsymmetricKeyParameter issuerPrivateKey = null)
        {
            // Generating Random Numbers
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom secureRandom = new SecureRandom(randomGenerator);
            
            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            
            // Serial Number
            BigInteger serialNumber =
                BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), secureRandom);
            certificateGenerator.SetSerialNumber(serialNumber);
            
            // Issuer and Subject Name
            X509Name subjectDn = new X509Name(subjectName);
            X509Name issuerDn = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDn);
            certificateGenerator.SetSubjectDN(subjectDn);
            
            certificateGenerator.SetNotBefore(validFrom);
            certificateGenerator.SetNotAfter(validTo);
            
            if (hostName != null)
            {
                // add subject alternative names
                Asn1Encodable[] subjectAlternativeNames = new Asn1Encodable[] { new GeneralName(GeneralName.DnsName, hostName) };

                DerSequence subjectAlternativeNamesExtension = new DerSequence(subjectAlternativeNames);
                certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName.Id, false,
                    subjectAlternativeNamesExtension);
            }
            
            // Subject Public Key
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(secureRandom, keyStrength);
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);
            
            // Set certificate intended purposes to only Server Authentication
            certificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage.Id, false,
                new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth));
            
            if (issuerPrivateKey == null)
            {
                certificateGenerator.AddExtension(X509Extensions.BasicConstraints.Id, true, new BasicConstraints(true));
            }
            
            Asn1SignatureFactory signatureFactory = new Asn1SignatureFactory(signatureAlgorithm,
                issuerPrivateKey ?? subjectKeyPair.Private, secureRandom);
            
            // Self-sign the certificate
            X509Certificate certificate = certificateGenerator.Generate(signatureFactory);
            
            // Corresponding private key
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);
            
            Asn1Sequence seq = (Asn1Sequence)Asn1Object.FromByteArray(privateKeyInfo.ParsePrivateKey().GetDerEncoded());
            
            if (seq.Count != 9)
            {
                throw new PemException("Malformed sequence in RSA private key");
            }
            
            RsaPrivateKeyStructure rsa = RsaPrivateKeyStructure.GetInstance(seq);
            RsaPrivateCrtKeyParameters rsaparams = new RsaPrivateCrtKeyParameters(rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent,
                rsa.Prime1, rsa.Prime2, rsa.Exponent1,
                rsa.Exponent2, rsa.Coefficient);
            
#if NET45
            // Set private key onto certificate instance
            X509Certificate2 x509Certificate = new X509Certificate2(certificate.GetEncoded());
            x509Certificate.PrivateKey = DotNetUtilities.ToRSA(rsaparams);
#else
            X509Certificate2 x509Certificate = withPrivateKey(certificate, rsaparams);
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                x509Certificate.FriendlyName = subjectName;    
            }
            
#endif
            
            doNotSetFriendlyName = false;
            
            if (!doNotSetFriendlyName)
            {
                try
                {
                    x509Certificate.FriendlyName = ProxyConstants.CNRemoverRegex.Replace(subjectName, string.Empty);
                }
                catch (System.PlatformNotSupportedException)
                {
                    doNotSetFriendlyName = true;
                }
            }
            
            return x509Certificate;
        }
        
        
        private static X509Certificate2 withPrivateKey(X509Certificate certificate, AsymmetricKeyParameter privateKey)
        {
            const string password = "password";
            Pkcs12Store store = new Pkcs12Store();
            X509CertificateEntry entry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(certificate.SubjectDN.ToString(), entry);
            
            store.SetKeyEntry(certificate.SubjectDN.ToString(), new AsymmetricKeyEntry(privateKey), new[] { entry });
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                store.Save(ms, password.ToCharArray(), new SecureRandom(new CryptoApiRandomGenerator()));
                
                return new X509Certificate2(ms.ToArray(), password, X509KeyStorageFlags.Exportable);
            }
        }
        
        
        /// <summary>
        ///     Makes the certificate internal.
        /// </summary>
        /// <param name="isRoot">if set to <c>true</c> [is root].</param>
        /// <param name="hostName">hostname for certificate</param>
        /// <param name="subjectName">The full subject.</param>
        /// <param name="validFrom">The valid from.</param>
        /// <param name="validTo">The valid to.</param>
        /// <param name="signingCertificate">The signing certificate.</param>
        /// <returns>X509Certificate2 instance.</returns>
        /// <exception cref="System.ArgumentException">
        ///     You must specify a Signing Certificate if and only if you are not creating a
        ///     root.
        /// </exception>
        private X509Certificate2 makeCertificateInternal(bool isRoot,
            string hostName, string subjectName,
            System.DateTime validFrom, System.DateTime validTo, X509Certificate2 signingCertificate)
        {
            if (isRoot != (null == signingCertificate))
            {
                throw new System.ArgumentException(
                    "You must specify a Signing Certificate if and only if you are not creating a root.",
                    nameof(signingCertificate));
            }
            
            if (isRoot)
            {
                return generateCertificate(null, subjectName, subjectName, validFrom, validTo);
            }
            
            AsymmetricCipherKeyPair kp = DotNetUtilities.GetKeyPair(signingCertificate.PrivateKey);
            return generateCertificate(hostName, subjectName, signingCertificate.Subject, validFrom, validTo,
                issuerPrivateKey: kp.Private);
        }
        
        
        /// <summary>
        ///     Makes the certificate internal.
        /// </summary>
        /// <param name="subject">The s subject cn.</param>
        /// <param name="isRoot">if set to <c>true</c> [is root].</param>
        /// <param name="switchToMtaIfNeeded">if set to <c>true</c> [switch to MTA if needed].</param>
        /// <param name="signingCert">The signing cert.</param>
        /// <param name="cancellationToken">Task cancellation token</param>
        /// <returns>X509Certificate2.</returns>
        private X509Certificate2 makeCertificateInternal(string subject, bool isRoot,
            bool switchToMtaIfNeeded, X509Certificate2 signingCert = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
#if NET45
            if (switchToMtaIfNeeded && Thread.CurrentThread.GetApartmentState() != ApartmentState.MTA)
            {
                X509Certificate2 certificate = null;
                using (ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false))
                {
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        try
                        {
                            certificate = makeCertificateInternal(subject, isRoot, false, signingCert);
                        }
                        catch (System.Exception ex)
                        {
                            exceptionFunc(new System.Exception("Failed to create BC certificate", ex));
                        }
                        
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            manualResetEvent.Set();
                        }
                    });
                    
                    manualResetEvent.Wait(System.TimeSpan.FromMinutes(1), cancellationToken);
                }
                
                return certificate;
            }
#endif
            
            return makeCertificateInternal(isRoot, subject, $"CN={subject}",
                System.DateTime.UtcNow.AddDays(-certificateGraceDays), System.DateTime.UtcNow.AddDays(certificateValidDays),
                isRoot ? null : signingCert);
        } // End Function makeCertificateInternal 
        
        
    } // End Class BCCertificateMaker 
    
    
} // End Namespace Titanium.Web.Proxy.Network.Certificate 
