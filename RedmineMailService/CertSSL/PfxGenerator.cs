
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Pkcs;


namespace AnySqlWebAdmin
{


    public class PfxGenerator
    {


        // System.Security.Cryptography.X509Certificates.X509Certificate2.Import (string fileName);

        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2.import?view=netframework-4.7.2
        // https://gist.github.com/yutopio/a217a4af63cf6bcf0a530c14c074cf8f
        // https://gist.githubusercontent.com/yutopio/a217a4af63cf6bcf0a530c14c074cf8f/raw/42b2f8cb27f6d22b7e22d65da5bbd0f1ce9b2fff/cert.cs
        // https://stackoverflow.com/questions/44755155/store-pkcs12-container-pfx-with-bouncycastle
        // https://github.com/Worlaf/RSADemo/blob/328692e28e48db92340d55563480c8724d916384/RSADemo_WinForms/frmRsaDemo.cs
        public static void CreatePfxFile(
              Org.BouncyCastle.X509.X509Certificate certificate
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey
            , string password
            , string keyFriendlyName)
        {
            // string password = System.Guid.NewGuid().ToString("N");

            // create certificate entry
            Org.BouncyCastle.Pkcs.X509CertificateEntry certEntry = 
                new Org.BouncyCastle.Pkcs.X509CertificateEntry(certificate);
            string friendlyName = certificate.SubjectDN.ToString();

            // get bytes of private key.
            Org.BouncyCastle.Asn1.Pkcs.PrivateKeyInfo keyInfo = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            byte[] keyBytes = keyInfo.ToAsn1Object().GetEncoded();

            Org.BouncyCastle.Pkcs.Pkcs12StoreBuilder builder = new Org.BouncyCastle.Pkcs.Pkcs12StoreBuilder();
            builder.SetUseDerEncoding(true);
            Org.BouncyCastle.Pkcs.Pkcs12Store store = builder.Build();

            // create store entry
            store.SetKeyEntry(
                  keyFriendlyName
                , new Org.BouncyCastle.Pkcs.AsymmetricKeyEntry(privateKey)
                , new Org.BouncyCastle.Pkcs.X509CertificateEntry[] {certEntry}
            );


            byte[] pfxBytes = null;

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Cert is contained in store
                // null: no password, "": an empty passwords
                store.Save(stream, password == null ? null: password.ToCharArray(), new Org.BouncyCastle.Security.SecureRandom());
                pfxBytes = stream.ToArray();
            } // End Using stream 

            byte[] result = Org.BouncyCastle.Pkcs.Pkcs12Utilities.ConvertToDefiniteLength(pfxBytes);
            // this.StoreCertificate(System.Convert.ToBase64String(result));

            string outputFilename = "example.pfx";
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(outputFilename, System.IO.FileMode.Create)))
            {
                writer.Write(result);
            } // End Using writer 

        }
        
        
    }
    
    
}
