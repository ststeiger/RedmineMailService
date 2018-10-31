
namespace RedmineMailService.CertSSL
{

    public abstract class NonBackdooredPrng : Org.BouncyCastle.Crypto.Prng.IRandomGenerator
    {
        public abstract void AddSeedMaterial(byte[] seed);
        public abstract void AddSeedMaterial(long seed);
        public abstract void NextBytes(byte[] bytes);
        public abstract void NextBytes(byte[] bytes, int start, int len);


        public static NonBackdooredPrng Create()
        {
            bool isWindows =
                System.Runtime.InteropServices.RuntimeInformation
                    .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

            if (isWindows)
                return new WindowsPrng();

            return new PosixPrng();
        } // End Function Create 


        public static Org.BouncyCastle.Security.SecureRandom SecureRandom
        {
            get
            {
                return new Org.BouncyCastle.Security.SecureRandom(
                    NonBackdooredPrng.Create()
                );
            }
        }


    } // End Class NonBackdooredPrng 



    public class WindowsPrng : NonBackdooredPrng
    {
        protected Org.BouncyCastle.Crypto.Prng.IRandomGenerator m_rnd;

        public WindowsPrng()
        {
            // this.m_rnd = new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator();

            const string digestName = "SHA256";
            Org.BouncyCastle.Crypto.IDigest digest = Org.BouncyCastle.Security.DigestUtilities.GetDigest(digestName);
            if (digest == null)
                return;

            Org.BouncyCastle.Crypto.Prng.DigestRandomGenerator prng =
                new Org.BouncyCastle.Crypto.Prng.DigestRandomGenerator(digest);

            const bool autoSeed = true;
            if (autoSeed)
            {
                // prng.AddSeedMaterial(NextCounterValue());
                // prng.AddSeedMaterial(GetNextBytes(Master, digest.GetDigestSize()));
            }

            this.m_rnd = prng;
        }


        /// <summary>Add more seed material to the generator.</summary>
        /// <param name="seed">A byte array to be mixed into the generator's state.</param>
        public override void AddSeedMaterial(byte[] seed)
        {
            this.m_rnd.AddSeedMaterial(seed);
        } // End Sub AddSeedMaterial 


        /// <summary>Add more seed material to the generator.</summary>
        /// <param name="seed">A long value to be mixed into the generator's state.</param>
        public override void AddSeedMaterial(long seed)
        {
            this.m_rnd.AddSeedMaterial(seed);
        } // End Sub AddSeedMaterial 


        /// <summary>Fill byte array with random values.</summary>
        /// <param name="bytes">Array to be filled.</param>
        public override void NextBytes(byte[] bytes)
        {
            this.m_rnd.NextBytes(bytes);
        } // End Sub NextBytes 


        /// <summary>Fill byte array with random values.</summary>
        /// <param name="bytes">Array to receive bytes.</param>
        /// <param name="start">Index to start filling at.</param>
        /// <param name="len">Length of segment to fill.</param>
        public override void NextBytes(byte[] bytes, int start, int len)
        {
            this.m_rnd.NextBytes(bytes, start, len);
        } // End Sub NextBytes 

    } // End Class WindowsPrng


    public class PosixPrng : NonBackdooredPrng
    {
        // Early boot on a very low entropy device. 
        // The /dev/urandom device cannot guarantee 
        // that it has received enough initial entropy, 
        // while when using /dev/random that is guaranteed 
        // (even if it may block).
        // therefore, use /dev/urandom

        // /dev/random //  potentially blocking
        // /dev/urandom


        /// <summary>Add more seed material to the generator.</summary>
        /// <param name="seed">A byte array to be mixed into the generator's state.</param>
        public override void AddSeedMaterial(byte[] seed)
        {
            // throw new System.NotImplementedException();
        } // End Sub AddSeedMaterial 


        /// <summary>Add more seed material to the generator.</summary>
        /// <param name="seed">A long value to be mixed into the generator's state.</param>
        public override void AddSeedMaterial(long seed)
        {
            // throw new System.NotImplementedException();
        } // End Sub AddSeedMaterial 


        /// <summary>Fill byte array with random values.</summary>
        /// <param name="bytes">Array to be filled.</param>
        public override void NextBytes(byte[] bytes)
        {
            using (System.IO.FileStream fs =
                new System.IO.FileStream(
                  "/dev/urandom"
                , System.IO.FileMode.Open
                , System.IO.FileAccess.Read))
            {
                fs.Read(bytes, 0, bytes.Length);
            }

        } // End Sub NextBytes 


        /// <summary>Fill byte array with random values.</summary>
        /// <param name="bytes">Array to receive bytes.</param>
        /// <param name="start">Index to start filling at.</param>
        /// <param name="len">Length of segment to fill.</param>
        public override void NextBytes(byte[] bytes, int start, int len)
        {
            using (System.IO.FileStream fs =
                new System.IO.FileStream(
                  "/dev/urandom"
                , System.IO.FileMode.Open
                , System.IO.FileAccess.Read))
            {
                fs.Read(bytes, start, len);
            }

        } // End Sub NextBytes 


    } // End Class LinuxPrng 


} // End Namespace CoreCMS.JWT 
