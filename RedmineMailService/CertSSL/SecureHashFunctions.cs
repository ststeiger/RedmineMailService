
namespace RedmineMailService.CertSSL
{


    public class SecureHashFunctions
    {


        // https://github.com/bcgit/bc-csharp/tree/096c4894f62b7f4178ab869ec342e9351bd198dd/crypto/src/crypto/digests
        public static byte[] Keccak(string text)
        {
            byte[] encData = System.Text.Encoding.UTF8.GetBytes(text);
            Org.BouncyCastle.Crypto.Digests.KeccakDigest myHash = new Org.BouncyCastle.Crypto.Digests.KeccakDigest();

            myHash.BlockUpdate(encData, 0, encData.Length);
            byte[] compArr = new byte[myHash.GetDigestSize()];
            myHash.DoFinal(compArr, 0);

            return compArr;
        } // End Function Keccak 


        // https://stackoverflow.com/questions/8674018/pbkdf2-with-bouncycastle-in-java
        public static byte[] PBKDF2(string text, byte[] salt, int iterations)
        {
            byte[] bytesToHash = System.Text.Encoding.UTF8.GetBytes(text);

            Org.BouncyCastle.Crypto.Generators.Pkcs5S2ParametersGenerator gen = new Org.BouncyCastle.Crypto.Generators.Pkcs5S2ParametersGenerator();
            gen.Init(Org.BouncyCastle.Crypto.PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(text.ToCharArray()), salt, iterations);

            // byte[] derivedKey = ((Org.BouncyCastle.Crypto.Parameters.KeyParameter)gen.GenerateDerivedParameters(160)).GetKey();
            byte[] derivedKey = ((Org.BouncyCastle.Crypto.Parameters.KeyParameter)gen.GenerateDerivedMacParameters(160)).GetKey();
            return derivedKey;
        } // End Function PBKDF2 


        public static byte[] BCrypt(string text, byte[] salt)
        {
            return BCrypt(text, salt, 4);
        } // End Function BCrypt 


        public static byte[] BCrypt(string text, byte[] salt, int cost)
        {
            byte[] bytesToHash = System.Text.Encoding.UTF8.GetBytes(text);

            byte[] hash = Org.BouncyCastle.Crypto.Generators.BCrypt.Generate(bytesToHash, salt, cost);
            return hash;
        } // End Function BCrypt 


        public static byte[] SCrypt(string text, byte[] salt, int cost)
        {
            byte[] bytesToHash = System.Text.Encoding.UTF8.GetBytes(text);

            byte[] hash = Org.BouncyCastle.Crypto.Generators.SCrypt.Generate(bytesToHash, salt, cost, 1024, 7, 32);
            return hash;
        } // End Function SCrypt 


        // https://github.com/bcgit/bc-csharp/tree/096c4894f62b7f4178ab869ec342e9351bd198dd/crypto/src/crypto/digests
        public static byte[] Sha3(string text)
        {
            byte[] encData = System.Text.Encoding.UTF8.GetBytes(text);
            Org.BouncyCastle.Crypto.Digests.Sha3Digest myHash = new Org.BouncyCastle.Crypto.Digests.Sha3Digest();

            myHash.BlockUpdate(encData, 0, encData.Length);
            byte[] compArr = new byte[myHash.GetDigestSize()];
            myHash.DoFinal(compArr, 0);

            return compArr;
        } // End Function Sha3 


    } // End Class SecureHashFunctions 


} // End Namespace RedmineMailService.CertSSL 
