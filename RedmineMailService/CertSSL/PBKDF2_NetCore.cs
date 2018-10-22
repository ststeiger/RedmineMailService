
namespace RedmineMailService.CertSSL
{

    // https://gist.github.com/atoponce/07d8d4c833873be2f68c34f9afc5a78a
    // https://paragonie.com/blog/2016/02/how-safely-store-password-in-2016
    // https://blog.novatec-gmbh.de/choosing-right-hashing-algorithm-slowness/
    // https://crypto.stackexchange.com/questions/27262/is-the-gost-block-cipher-broken
    // http://javadoc.iaik.tugraz.at/iaik_jce/current/iaik/security/md/GOST3411.html
    // nearly everyone except FIPS agrees PBKDF2 is the worst of the acceptable options, but is still acceptable
    class PBKDF2_NetCore
    {

        public const int SaltByteSize = 24;
        public const int HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        public const int Pbkdf2Iterations = 1000;
        public const int IterationIndex = 0;
        public const int SaltIndex = 1;
        public const int Pbkdf2Index = 2;


        public static string HashPassword(string password)
        {
            byte[] hash = null;
            byte[] salt = null;

            using (System.Security.Cryptography.RNGCryptoServiceProvider cryptoProvider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                salt = new byte[SaltByteSize];
                cryptoProvider.GetBytes(salt);

                hash = GetPbkdf2Bytes(password, salt, Pbkdf2Iterations, HashByteSize);
            }

            return Pbkdf2Iterations + ":" +
                   System.Convert.ToBase64String(salt) + ":" +
                   System.Convert.ToBase64String(hash);
        }


        public static bool ValidatePassword(string password, string correctHash)
        {
            char[] delimiter = { ':' };
            string[] split = correctHash.Split(delimiter);
            int iterations = int.Parse(split[IterationIndex]);
            byte[] salt = System.Convert.FromBase64String(split[SaltIndex]);
            byte[] hash = System.Convert.FromBase64String(split[Pbkdf2Index]);

            byte[] testHash = GetPbkdf2Bytes(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }


        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }


        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            byte[] hashBytes = null;

            using (System.Security.Cryptography.Rfc2898DeriveBytes pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                hashBytes = pbkdf2.GetBytes(outputBytes);
            } // End Using pbkdf2 

            return hashBytes;
        }


    } // End Class PBKDF2_NetCore 


} // End Namespace RedmineMailService.CertSSL
