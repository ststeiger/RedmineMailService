
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;


// https://github.com/bcgit/bc-csharp/blob/master/crypto/src/security/DigestUtilities.cs
// https://github.com/bcgit/bc-csharp/blob/master/crypto/src/security/SignerUtilities.cs
// https://stackoverflow.com/questions/47476427/get-a-list-of-all-supported-digest-algorithms
// https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/operators/Asn1Signature.cs
// https://github.com/kerryjiang/BouncyCastle.Crypto/blob/master/Crypto/x509/X509Utilities.cs
// https://github.com/bcgit/bc-csharp/blob/master/crypto/src/security/GeneratorUtilities.cs

// RIPEMD* derivates of MD4
// SHA256WITHRSAANDMGF1
// GOST3411WITHECGOST3410-2001 // ECGOST3410, GOST3410, GOST3410-94, GOST3410-2001
namespace RedmineMailService.CertSSL
{

    // Class that contains a map with the different message digest algorithms.
    public static class DigestAlgorithms
    {

        
        // public const string SHA1 = "SHA-1";
        public const string SHA256 = "SHA-256";
        public const string SHA384 = "SHA-384";
        public const string SHA512 = "SHA-512";

        // RIPEMD-160 is an old algorithm that has been deprecated by all major security solutions in favor of more modern algorithms.
        // And although there is no publicly known attack against it, its aging design coupled with new advances in attacks techniques 
        // make it risky to continue relying on it.
        public const string RIPEMD160 = "RIPEMD160";

        // The first cryptographic break for the GOST hash function was published in 2008. See cryptanalysis for more information.

        public const string GOST3411 = "GOST3411";

        // The first cryptanalysis on a reduced version of the WHIRLPOOL hash function 
        // (number of rounds R less than 10) was published in 2009. 
        // There is no known attack on the full WHIRLPOOL hash function (10 rounds).



        public static IDigest GetMessageDigestByOid(string digestOid)
        {
            return DigestUtilities.GetDigest(digestOid);
        }

        
        public static IDigest GetMessageDigestByName(string hashAlgorithm)
        {
            return DigestUtilities.GetDigest(hashAlgorithm);
        }


        public static byte[] ComputeHash(System.IO.Stream data, IDigest messageDigest)
        {
            byte[] buf = new byte[8192];
            int n;
            while ((n = data.Read(buf, 0, buf.Length)) > 0)
            {
                messageDigest.BlockUpdate(buf, 0, n);
            }

            byte[] r = new byte[messageDigest.GetDigestSize()];
            messageDigest.DoFinal(r, 0);
            return r;
        }


        public static byte[] ComputeHash(System.IO.Stream data, string hashAlgorithm)
        {
            IDigest messageDigest = GetMessageDigestByName(hashAlgorithm);
            return ComputeHash(data, messageDigest);
        }


        public static byte[] ComputeHash(IDigest d, byte[] input, int offset, int len)
        {
            d.BlockUpdate(input, offset, len);
            byte[] r = new byte[d.GetDigestSize()];
            d.DoFinal(r, 0);
            return r;
        }


        public static byte[] ComputeHash(IDigest d, byte[] input)
        {
            return ComputeHash(d, input, 0, input.Length);
        }


        public static byte[] ComputeHash(string algo, byte[] input, int offset, int len)
        {
            return ComputeHash(DigestUtilities.GetDigest(algo), input, offset, len);
        }


        public static byte[] ComputeHash(string algo, byte[] input)
        {
            return ComputeHash(DigestUtilities.GetDigest(algo), input, 0, input.Length);
        }
        

    }


}
