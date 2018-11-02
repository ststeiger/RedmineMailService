
namespace RedmineMailService.CertSSL
{


    // https://gist.github.com/therightstuff/aa65356e95f8d0aae888e9f61aa29414
    public class KeyImportExport
    {


        // KeyImportExport.GetPemKeyPair
        public static PrivatePublicPemKeyPair GetPemKeyPair(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair)
        {
            PrivatePublicPemKeyPair result = new PrivatePublicPemKeyPair();
            
            // id_rsa
            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();

                result.PrivateKey = textWriter.ToString();
            } // End Using textWriter 


            // id_rsa.pub
            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(keyPair.Public);
                pemWriter.Writer.Flush();

                result.PublicKey = textWriter.ToString();
            } // End Using textWriter 


            // // This writes the same as private key, not both
            //using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            //{
            //    Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
            //    pemWriter.WriteObject(keyPair);
            //    pemWriter.Writer.Flush();

            //    bothKeys = textWriter.ToString();
            //} // End Using textWriter 

            return result;
        } // End Sub GetPemKeyPair


        // KeyImportExport.ReadPublicKey
        public static Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadPublicKey(string publicKey)
        {
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter keyParameter = null;

            using (System.IO.TextReader reader = new System.IO.StringReader(publicKey))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader =
                    new Org.BouncyCastle.OpenSsl.PemReader(reader);

                object obj = pemReader.ReadObject();

                if ((obj is Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair))
                    throw new System.ArgumentException("The given publicKey is actually a private key.", "publicKey");

                if (!(obj is Org.BouncyCastle.Crypto.AsymmetricKeyParameter))
                    throw new System.ArgumentException("The given publicKey is not a valid assymetric key.", "publicKey");

                keyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)obj;
            }

            return keyParameter;
        } // End Function ReadPublicKey 



        public static Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadPrivateKey(string privateKey)
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = null;

            using (System.IO.TextReader reader = new System.IO.StringReader(privateKey))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader =
                    new Org.BouncyCastle.OpenSsl.PemReader(reader);

                object obj = pemReader.ReadObject();

                if (obj is Org.BouncyCastle.Crypto.AsymmetricKeyParameter)
                    throw new System.ArgumentException("The given privateKey is a public key, not a privateKey...", "privateKey");

                if (!(obj is Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair))
                    throw new System.ArgumentException("The given privateKey is not a valid assymetric key.", "privateKey");

                keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)obj;
            } // End using reader 

            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = keyPair.Private;
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter pub = keyPair.Public;

            // Note: 
            // cipher.Init(false, key);
            // !!!

            return keyPair.Private;
        } // End Function ReadPrivateKey


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair ReadKeyPair(string privateKey)
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = null;

            using (System.IO.TextReader reader = new System.IO.StringReader(privateKey))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader =
                    new Org.BouncyCastle.OpenSsl.PemReader(reader);

                object obj = pemReader.ReadObject();

                if (obj is Org.BouncyCastle.Crypto.AsymmetricKeyParameter)
                    throw new System.ArgumentException("The given privateKey is a public key, not a privateKey...", "privateKey");

                if (!(obj is Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair))
                    throw new System.ArgumentException("The given privateKey is not a valid assymetric key.", "privateKey");

                keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)obj;
            }

            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = keyPair.Private;
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter pub = keyPair.Public;

            // Note: 
            // cipher.Init(false, key);
            // !!!

            return keyPair;
        } // End Function ReadPrivateKey


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair ReadKeyPairFromFile(string fileName)
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair KeyPair = null;

            //  Stream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (System.IO.FileStream fs = System.IO.File.OpenRead(fileName))
            {

                using (System.IO.StreamReader sr = new System.IO.StreamReader(fs))
                {
                    Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                    KeyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)pemReader.ReadObject();
                    // System.Security.Cryptography.RSAParameters rsa = Org.BouncyCastle.Security.
                    //     DotNetUtilities.ToRSAParameters((Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)KeyPair.Private);
                } // End Using sr 

            } // End Using fs 

            return KeyPair;
        } // End Function ImportKeyPair 


        //public static void ReadPrivateKeyFile(string privateKeyFileName)
        //{
        //    Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters key = null;

        //    using (System.IO.StreamReader streamReader = System.IO.File.OpenText(privateKeyFileName))
        //    {
        //        Org.BouncyCastle.OpenSsl.PemReader pemReader = 
        //            new Org.BouncyCastle.OpenSsl.PemReader(streamReader);
        //        key = (Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters) pemReader.ReadObject();
        //    } // End Using streamReader 

        //    // Note: 
        //    // cipher.Init(false, key);
        //    // !!!
        //} // End Function ReadPrivateKey

        
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadPublicKeyFile(string pemFilename)
        {
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter keyParameter = null;

            using (System.IO.StreamReader streamReader = System.IO.File.OpenText(pemFilename))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(streamReader);
                keyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)pemReader.ReadObject();
            } // End Using fileStream 

            return keyParameter;
        } // End Function ReadPublicKey 


        public static void ExportKeyPair(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair)
        {
            string privateKey = null;

            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();

                privateKey = textWriter.ToString();
            } // End Using textWriter 

            System.Console.WriteLine(privateKey);
        } // End Sub ExportKeyPair 


        // https://stackoverflow.com/questions/22008337/generating-keypair-using-bouncy-castle
        // https://stackoverflow.com/questions/14052485/converting-a-public-key-in-subjectpublickeyinfo-format-to-rsapublickey-format-ja
        // https://stackoverflow.com/questions/10963756/get-der-encoded-public-key
        // http://www.programcreek.com/java-api-examples/index.php?api=org.bouncycastle.crypto.util.SubjectPublicKeyInfoFactory
        public static void CerKeyInfo(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair)
        {
            Org.BouncyCastle.Asn1.Pkcs.PrivateKeyInfo pkInfo = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            string privateKey = System.Convert.ToBase64String(pkInfo.GetDerEncoded());

            // and following for public:
            Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo info = Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            string publicKey = System.Convert.ToBase64String(info.GetDerEncoded());

            System.Console.WriteLine(privateKey);
            System.Console.WriteLine(publicKey);
        } // End Sub CerKeyInfo 


    } // End Class KeyImportExport 


} // End Namespace RedmineMailService.CertSSL 
