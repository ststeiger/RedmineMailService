
namespace AnySqlWebAdmin
{
    
    
    public class PfxGenerator
    {


        public static System.Security.Cryptography.X509Certificates.X509Certificate2
            LoadCertificate(string pfxLocation, string password)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert =
                    new System.Security.Cryptography.X509Certificates.X509Certificate2(
                          pfxLocation
                        , password);

            return cert;
        }
        

        public static System.Security.Cryptography.X509Certificates.X509Certificate2
            LoadCertificate(string pfxLocation)
        {
            return LoadCertificate(pfxLocation, "");
        }


        public static Org.BouncyCastle.X509.X509Certificate LoadPfx(string path, string password)
        {
            Org.BouncyCastle.X509.X509Certificate cert = null;
            Org.BouncyCastle.Pkcs.X509CertificateEntry ce = null;

            using (System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                Org.BouncyCastle.Pkcs.Pkcs12Store store = new Org.BouncyCastle.Pkcs.Pkcs12Store(fs, password.ToCharArray());

                foreach (string thisAlias in store.Aliases)
                {
                    System.Console.WriteLine(thisAlias);

                    ce = store.GetCertificate(thisAlias);
                    cert = ce.Certificate;
                    break;
                } // Next thisAlias 

            } // End Using fs 

            System.Console.WriteLine(cert);
            return cert;
        } // End Function LoadPfx 


        public static Org.BouncyCastle.X509.X509Certificate LoadPfx(string path)
        {
            return LoadPfx(path, "");
        } // End Function LoadPfx 






        // System.Security.Cryptography.X509Certificates.X509Certificate2.Import (string fileName);

        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2.import?view=netframework-4.7.2
        // https://gist.github.com/yutopio/a217a4af63cf6bcf0a530c14c074cf8f
        // https://gist.githubusercontent.com/yutopio/a217a4af63cf6bcf0a530c14c074cf8f/raw/42b2f8cb27f6d22b7e22d65da5bbd0f1ce9b2fff/cert.cs
        // https://stackoverflow.com/questions/44755155/store-pkcs12-container-pfx-with-bouncycastle
        // https://github.com/Worlaf/RSADemo/blob/328692e28e48db92340d55563480c8724d916384/RSADemo_WinForms/frmRsaDemo.cs
        public static void CreatePfxFile(
              string fileName
            , Org.BouncyCastle.X509.X509Certificate certificate
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey
            , string password)
        {
            // string password = System.Guid.NewGuid().ToString("N");
            
            // create certificate entry
            Org.BouncyCastle.Pkcs.X509CertificateEntry certEntry = 
                new Org.BouncyCastle.Pkcs.X509CertificateEntry(certificate);
            string friendlyName = certificate.SubjectDN.ToString();
            
            
            
            // get bytes of private key.
            Org.BouncyCastle.Asn1.Pkcs.PrivateKeyInfo keyInfo = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            //byte[] keyBytes = keyInfo.ToAsn1Object().GetEncoded();
            
            Org.BouncyCastle.Pkcs.Pkcs12StoreBuilder builder = new Org.BouncyCastle.Pkcs.Pkcs12StoreBuilder();
            builder.SetUseDerEncoding(true);
            
            
            
            Org.BouncyCastle.Pkcs.Pkcs12Store store = builder.Build();
            
            store.SetCertificateEntry(friendlyName, certEntry);
            
            // create store entry
            store.SetKeyEntry(
                  //keyFriendlyName
                  friendlyName
                , new Org.BouncyCastle.Pkcs.AsymmetricKeyEntry(privateKey)
                , new Org.BouncyCastle.Pkcs.X509CertificateEntry[] {certEntry}
            );
            
            
            byte[] pfxBytes = null;
            
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Cert is contained in store
                // null: no password, "": an empty passwords
                // note: Linux needs empty password on null...
                store.Save(stream, password == null ? "".ToCharArray(): password.ToCharArray(), new Org.BouncyCastle.Security.SecureRandom());
                // stream.Position = 0;
                pfxBytes = stream.ToArray();
            } // End Using stream 
            
            
#if WITH_MS_PFX 
            WithMsPfx(pfxBytes, fileName, password);
#else
            byte[] result = Org.BouncyCastle.Pkcs.Pkcs12Utilities.ConvertToDefiniteLength(pfxBytes);
            // this.StoreCertificate(System.Convert.ToBase64String(result));
            
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(fileName, System.IO.FileMode.Create)))
            {
                writer.Write(result);
            } // End Using writer 
#endif
            
        }
        
        
        public static void WithMsPfx(byte[] pfxBytes, string fileName, string password)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 convertedCertificate =
                new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxBytes,
                            "", // PW
                            System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet | System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable);
            
            byte[] bytes = convertedCertificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pfx, password);
            System.IO.File.WriteAllBytes(fileName, bytes);
        }
        
        
    }
    
    
}
