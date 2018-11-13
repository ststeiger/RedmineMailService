
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


        // https://stackoverflow.com/questions/21565369/how-to-encrypt-and-salt-the-password-using-bouncycastle-api-in-java
        // Password-Based Key Derivation Function
        // PBKDF1 and PBKDF2 
        public static void PBKDF2(string passwordToSave, string passwordToCheck)
        {
            // tuning parameters

            // these sizes are relatively arbitrary
            int seedBytes = 20;
            int hashBytes = 20;

            // increase iterations as high as your performance can tolerate
            // since this increases computational cost of password guessing
            // which should help security
            int iterations = 1000;

            // to save a new password:

            Org.BouncyCastle.Security.SecureRandom rng = new Org.BouncyCastle.Security.SecureRandom();


            byte[] salt = rng.GenerateSeed(seedBytes);

            Org.BouncyCastle.Crypto.Generators.Pkcs5S2ParametersGenerator kdf = new Org.BouncyCastle.Crypto.Generators.Pkcs5S2ParametersGenerator();
            kdf.Init(System.Text.Encoding.UTF8.GetBytes(passwordToSave), salt, iterations);

            byte[] hash =
                ((Org.BouncyCastle.Crypto.Parameters.KeyParameter)kdf.GenerateDerivedMacParameters(8 * hashBytes)).GetKey();

            // now save salt and hash

            // to check a password, given the known previous salt and hash:

            kdf = new Org.BouncyCastle.Crypto.Generators.Pkcs5S2ParametersGenerator();
            kdf.Init(System.Text.Encoding.UTF8.GetBytes(passwordToCheck), salt, iterations);

            byte[] hashToCheck =
                ((Org.BouncyCastle.Crypto.Parameters.KeyParameter)kdf.GenerateDerivedMacParameters(8 * hashBytes)).GetKey();

            // if the bytes of hashToCheck don't match the bytes of hash
            // that means the password is invalid
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




        // https://www.programcreek.com/java-api-examples/index.php?api=org.bouncycastle.crypto.engines.SerpentEngine
        // https://stackoverflow.com/questions/9806012/multiple-encryption-with-bouncycastle
        public static void Poly1305Tests()
        {
            // https://www.programcreek.com/java-api-examples/index.php?api=org.bouncycastle.crypto.engines.TwofishEngine
            var poly = new Org.BouncyCastle.Crypto.Macs.Poly1305(new Org.BouncyCastle.Crypto.Engines.SerpentEngine());
            poly.Init(null);


            var se = new Org.BouncyCastle.Crypto.Engines.Salsa20Engine();
            var pwiv = new Org.BouncyCastle.Crypto.Parameters.ParametersWithIV(
                  new Org.BouncyCastle.Crypto.Parameters.KeyParameter(new byte[16])
                , new byte[8]
            );


            // https://en.wikipedia.org/wiki/Salsa20
            var xse = new Org.BouncyCastle.Crypto.Engines.XSalsa20Engine();
            var xsekp = new Org.BouncyCastle.Crypto.Parameters.ParametersWithIV(
                new Org.BouncyCastle.Crypto.Parameters.KeyParameter(new byte[32])
                , new byte[24]
            );


            var engine = new Org.BouncyCastle.Crypto.Engines.RC4Engine();
            byte[] key = System.Text.Encoding.UTF8.GetBytes("foobar");
            engine.Init(true, new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key));


            Org.BouncyCastle.Crypto.Engines.TwofishEngine twofish = new Org.BouncyCastle.Crypto.Engines.TwofishEngine();
            twofish.Init(true, new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key));
            new Org.BouncyCastle.Crypto.Engines.ThreefishEngine(1024);

            Org.BouncyCastle.Crypto.BufferedBlockCipher cipher =
                new Org.BouncyCastle.Crypto.BufferedBlockCipher(new Org.BouncyCastle.Crypto.Engines.TwofishEngine());


        }

        private void DoEax(byte[] key, byte[] iv, byte[] pt, byte[] aad, int tagLength, byte[] expected)
        {
            Org.BouncyCastle.Crypto.Modes.EaxBlockCipher c = new Org.BouncyCastle.Crypto.Modes.EaxBlockCipher(new Org.BouncyCastle.Crypto.Engines.SerpentEngine());

            c.Init(true, new Org.BouncyCastle.Crypto.Parameters.AeadParameters(
                new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key), tagLength, iv, aad)
            );

            byte[] outBytes = new byte[expected.Length];

            int len = c.ProcessBytes(pt, 0, pt.Length, outBytes, 0);
            c.DoFinal(outBytes, len);

            if (!Org.BouncyCastle.Utilities.Arrays.AreEqual(expected, outBytes))
            {
                System.Console.WriteLine("EAX test failed");
            }
        }
    

        private void DoCbc(byte[] key, byte[] iv, byte[] pt, byte[] expected)
        {
            Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher c =
                new Org.BouncyCastle.Crypto.Paddings.PaddedBufferedBlockCipher(
                    new Org.BouncyCastle.Crypto.Modes.CbcBlockCipher(
                        new Org.BouncyCastle.Crypto.Engines.SerpentEngine())
                        , new Org.BouncyCastle.Crypto.Paddings.Pkcs7Padding());

            byte[] ct = new byte[expected.Length];

            c.Init(true, new Org.BouncyCastle.Crypto.Parameters.ParametersWithIV(
                new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key), iv));

            int l = c.ProcessBytes(pt, 0, pt.Length, ct, 0);

            c.DoFinal(ct, l);

            if (!Org.BouncyCastle.Utilities.Arrays.AreEqual(expected, ct))
            {
                System.Console.WriteLine("CBC test failed");
            }
        }


        private void Poly1305CheckHash(byte[] input, byte[] tag)
        {
            Org.BouncyCastle.Crypto.Generators.Poly1305KeyGenerator keygen = new Org.BouncyCastle.Crypto.Generators.Poly1305KeyGenerator();
            keygen.Init(new Org.BouncyCastle.Crypto.KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), 1024));
            byte[] key = keygen.GenerateKey();
            Poly1305CheckHash(key, input, tag);
        }


        private void Poly1305CheckHash(byte[] keyMaterial, byte[] input, byte[] tag)
        {
            Org.BouncyCastle.Crypto.Macs.Poly1305 poly1305 = new Org.BouncyCastle.Crypto.Macs.Poly1305();
            poly1305.Init(new Org.BouncyCastle.Crypto.Parameters.KeyParameter(keyMaterial));

            poly1305.BlockUpdate(input, 0, input.Length);

            byte[] mac = new byte[poly1305.GetMacSize()];

            poly1305.DoFinal(mac, 0);

            if (!Org.BouncyCastle.Utilities.Arrays.AreEqual(tag, mac))
            {
                System.Console.WriteLine("fail");
                // Fail("rfc7539", Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(tag), Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(mac));
            }
        }


        // https://csharp.hotexamples.com/examples/Org.BouncyCastle.Crypto.Macs/Poly1305/BlockUpdate/php-poly1305-blockupdate-method-examples.html
        public static void Poly1305AES()
        {
            Org.BouncyCastle.Crypto.IMac mac = new Org.BouncyCastle.Crypto.Macs.Poly1305(new Org.BouncyCastle.Crypto.Engines.AesEngine());
            byte[] key = null;
            byte[] n = new byte[16];

            byte[] input = System.Text.Encoding.UTF8.GetBytes("Hello World!");
            byte[] output = new byte[mac.GetMacSize()];

            for (int loop = 0; loop < 13; loop++)
            {
                mac.Init(new Org.BouncyCastle.Crypto.Parameters.ParametersWithIV(new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key), n));
                mac.BlockUpdate(input, 0, input.Length);
                mac.DoFinal(output, 0);
            }
        }


        public static void NeoGmac()
        {

            byte[] input = System.Text.Encoding.UTF8.GetBytes("Hello World!");


            byte[] keyMaterial = null;
            var nk = new Org.BouncyCastle.Crypto.Engines.NoekeonEngine();
            // nk.Init(true, new Org.BouncyCastle.Crypto.Parameters.KeyParameter(keyMaterial));
            var mac = new Org.BouncyCastle.Crypto.Macs.GMac(new Org.BouncyCastle.Crypto.Modes.GcmBlockCipher(nk));
            byte[] output = new byte[mac.GetMacSize()];

            mac.Init(new Org.BouncyCastle.Crypto.Parameters.KeyParameter(keyMaterial));
            mac.BlockUpdate(input, 0, input.Length);
            mac.DoFinal(output, 0);
        }


    } // End Class SecureHashFunctions 


} // End Namespace RedmineMailService.CertSSL 
