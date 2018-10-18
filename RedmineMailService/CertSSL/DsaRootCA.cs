using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.X509.Store;


using System.Collections.Generic;

using System.Linq;
using System.Security.Cryptography;



// https://github.com/compliashield/certificate-issuer
// https://github.com/KimikoMuffin/bc-csharp
// https://github.com/bcgit/bc-csharp/blob/master/crypto/test/src/pkcs/test/PKCS12StoreTest.cs


// https://csharp.hotexamples.com/examples/Org.BouncyCastle.Pkcs/Pkcs12StoreBuilder/Load/php-pkcs12storebuilder-load-method-examples.html


namespace RedmineMailService.CertSSL
{



    class DsaRootCA
    {



        static IEnumerable<Org.BouncyCastle.X509.X509Certificate> BuildCertificateChainBC(byte[] primary, IEnumerable<byte[]> additional)
        {
            X509CertificateParser parser = new X509CertificateParser();
            PkixCertPathBuilder builder = new PkixCertPathBuilder();

            // Separate root from itermediate
            List<Org.BouncyCastle.X509.X509Certificate> intermediateCerts = new List<Org.BouncyCastle.X509.X509Certificate>();
            HashSet rootCerts = new HashSet();

            foreach (byte[] cert in additional)
            {
                Org.BouncyCastle.X509.X509Certificate x509Cert = parser.ReadCertificate(cert);

                // Separate root and subordinate certificates
                if (x509Cert.IssuerDN.Equivalent(x509Cert.SubjectDN))
                    rootCerts.Add(new TrustAnchor(x509Cert, null));
                else
                    intermediateCerts.Add(x509Cert);
            }

            // Create chain for this certificate
            X509CertStoreSelector holder = new X509CertStoreSelector();
            holder.Certificate = parser.ReadCertificate(primary);

            // WITHOUT THIS LINE BUILDER CANNOT BEGIN BUILDING THE CHAIN
            intermediateCerts.Add(holder.Certificate);

            PkixBuilderParameters builderParams = new PkixBuilderParameters(rootCerts, holder);
            builderParams.IsRevocationEnabled = false;

            X509CollectionStoreParameters intermediateStoreParameters =
                new X509CollectionStoreParameters(intermediateCerts);

            builderParams.AddStore(X509StoreFactory.Create(
                "Certificate/Collection", intermediateStoreParameters));

            PkixCertPathBuilderResult result = builder.Build(builderParams);

            return result.CertPath.Certificates.Cast<Org.BouncyCastle.X509.X509Certificate>();
        }

        private static AsymmetricCipherKeyPair GenerateDsaKeys()
        {
            DSACryptoServiceProvider DSA = new DSACryptoServiceProvider();
            DSAParameters dsaParams = DSA.ExportParameters(true);
            AsymmetricCipherKeyPair keys = Org.BouncyCastle.Security.DotNetUtilities.GetDsaKeyPair(dsaParams);
            return keys;
        }


        // -------------------------


        
        private static string _certificatesDir = @"C:\Certificates";

        public void IssueClientFromCA()
        {
            // get CA
            string caCn = "MyCA CommonName";
            char[] caPass = "passwordForThePfx".ToCharArray();

            using (System.IO.Stream caCertFile = System.IO.File.OpenRead(string.Format(@"{0}\{1}", _certificatesDir, "MyCAFile.pfx")))
            {
                Pkcs12Store store = new Pkcs12StoreBuilder().Build();
                store.Load(caCertFile, caPass);
                Org.BouncyCastle.X509.X509Certificate caCert = store.GetCertificate(caCn).Certificate;
                AsymmetricKeyParameter caPrivKey = store.GetKey(caCn).Key;

                byte[] clientCert = GenerateDsaCertificateAsPkcs12(
                    "My Client FriendlyName",
                    "My Client SubjectName",
                    "GT",
                    new System.DateTime(2011, 9, 19),
                    new System.DateTime(2014, 9, 18),
                    "PFXPASS",
                    caCert,
                    caPrivKey);

                string saveAS = string.Format(@"{0}\{1}", _certificatesDir, "clientCertFile.pfx");
                System.IO.File.WriteAllBytes(saveAS, clientCert);
            }

        }

        public static byte[] GenerateDsaCertificateAsPkcs12(
            string friendlyName,
            string subjectName,
            string country,
            System.DateTime validStartDate,
            System.DateTime validEndDate,
            string password,
            Org.BouncyCastle.X509.X509Certificate caCert,
            AsymmetricKeyParameter caPrivateKey)
        {
            AsymmetricCipherKeyPair keys = GenerateDsaKeys();

            #region build certificate
            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();

            // build name attributes
            System.Collections.ArrayList nameOids = new System.Collections.ArrayList();
            nameOids.Add(Org.BouncyCastle.Asn1.X509.X509Name.CN);
            nameOids.Add(X509Name.O);
            nameOids.Add(X509Name.C);

            System.Collections.ArrayList nameValues = new System.Collections.ArrayList();
            nameValues.Add(friendlyName);
            nameValues.Add(subjectName);
            nameValues.Add(country);
            X509Name subjectDN = new X509Name(nameOids, nameValues);

            // certificate fields
            certGen.SetSerialNumber(BigInteger.ValueOf(1));
            certGen.SetIssuerDN(caCert.SubjectDN);
            certGen.SetNotBefore(validStartDate);
            certGen.SetNotAfter(validEndDate);
            certGen.SetSubjectDN(subjectDN);
            certGen.SetPublicKey(keys.Public);
            certGen.SetSignatureAlgorithm("SHA1withDSA");

            // extended information
            certGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(caCert.GetPublicKey()));
            certGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(keys.Public));
            #endregion

            // generate x509 certificate
            X509Certificate cert = certGen.Generate(caPrivateKey);
            //ert.Verify(caCert.GetPublicKey());

            Dictionary<string, Org.BouncyCastle.X509.X509Certificate> chain = new Dictionary<string, Org.BouncyCastle.X509.X509Certificate>();
            //chain.Add("CertiFirmas CA", caCert);

            // string caCn = caCert.SubjectDN.GetValues(X509Name.CN)[0].ToString();
            string caCn = caCert.SubjectDN.GetValueList(X509Name.CN)[0].ToString();
            chain.Add(caCn, caCert);

            // store the file
            return GeneratePkcs12(keys, cert, friendlyName, password, chain);
        }

        private static byte[] GeneratePkcs12(AsymmetricCipherKeyPair keys, Org.BouncyCastle.X509.X509Certificate cert, string friendlyName, string password,
            Dictionary<string, Org.BouncyCastle.X509.X509Certificate> chain)
        {
            List<X509CertificateEntry> chainCerts = new List<X509CertificateEntry>();

            // Create the PKCS12 store
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();

            // Add a Certificate entry
            X509CertificateEntry certEntry = new X509CertificateEntry(cert);
            store.SetCertificateEntry(friendlyName, certEntry); // use DN as the Alias.
                                                                //chainCerts.Add(certEntry);

            // Add chain entries
            List<byte[]> additionalCertsAsBytes = new List<byte[]>();
            if (chain != null && chain.Count > 0)
            {
                foreach (KeyValuePair<string, X509Certificate> additionalCert in chain)
                {
                    additionalCertsAsBytes.Add(additionalCert.Value.GetEncoded());
                }
            }

            if (chain != null && chain.Count > 0)
            {
                IEnumerable<X509Certificate> addicionalCertsAsX09Chain = BuildCertificateChainBC(cert.GetEncoded(), additionalCertsAsBytes);

                foreach (X509Certificate addCertAsX09 in addicionalCertsAsX09Chain)
                {
                    chainCerts.Add(new X509CertificateEntry(addCertAsX09));
                }
            }

            // Add a key entry
            AsymmetricKeyEntry keyEntry = new AsymmetricKeyEntry(keys.Private);

            // no chain
            store.SetKeyEntry(friendlyName, keyEntry, new X509CertificateEntry[] { certEntry });

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                store.Save(memoryStream, password.ToCharArray(), new SecureRandom());
                return memoryStream.ToArray();
            }
        }


    }
}
