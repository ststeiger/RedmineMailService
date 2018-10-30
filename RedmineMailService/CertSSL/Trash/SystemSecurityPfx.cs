
namespace RedmineMailService.CertSSL.Trash
{


    internal class SystemSecurityPfx
    {


        // DumpPfx(ee25519Cert, subject, caKey25519);
        public static void DumpPfx(
              Org.BouncyCastle.X509.X509Certificate bouncyCert
            , Org.BouncyCastle.Asn1.X509.X509Name subject
            , Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair pair)
        {
            Org.BouncyCastle.Pkcs.Pkcs12Store store = new Org.BouncyCastle.Pkcs.Pkcs12Store();
            Org.BouncyCastle.Pkcs.X509CertificateEntry certificateEntry =
                new Org.BouncyCastle.Pkcs.X509CertificateEntry(bouncyCert);

            store.SetCertificateEntry(subject.ToString(), certificateEntry);

            store.SetKeyEntry(subject.ToString(),
                  new Org.BouncyCastle.Pkcs.AsymmetricKeyEntry(pair.Private)
                , new[] { certificateEntry }
            );

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
                } // End Using cert 

            } // End Using stream 

        } // End Sub DumpPfx 


    }


}
