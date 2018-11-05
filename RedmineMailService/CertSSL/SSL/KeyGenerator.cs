﻿
namespace RedmineMailService.CertSSL
{


    public class KeyGenerator
    {

        public static void foo()
        {
            Org.BouncyCastle.Crypto.IAsymmetricCipherKeyPairGenerator gen = Org.BouncyCastle.Security.GeneratorUtilities.GetKeyPairGenerator("");
        }



        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateGhostKeyPair(
            int length
           , Org.BouncyCastle.Security.SecureRandom secureRandom
           )
        {
            Org.BouncyCastle.Crypto.KeyGenerationParameters keygenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, length);

            Org.BouncyCastle.Crypto.Generators.Gost3410KeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.Gost3410KeyPairGenerator();
            keyGenerator.Init(keygenParam);

            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateRsaKeyPair 




        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(
             int length
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Crypto.KeyGenerationParameters keygenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, length);

            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
            keyGenerator.Init(keygenParam);

            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateRsaKeyPair 



        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateDsaKeyPair(
             int length
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Crypto.KeyGenerationParameters keygenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, length);

            Org.BouncyCastle.Crypto.Generators.DsaKeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.DsaKeyPairGenerator();
            keyGenerator.Init(keygenParam);

            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateRsaKeyPair 


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcKeyPair(
              Org.BouncyCastle.Asn1.X9.X9ECParameters ecParam
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters ecDomain =
                new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(ecParam.Curve, ecParam.G, ecParam.N);

            Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters keygenParam =
                new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(ecDomain, secureRandom);

            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();

            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateEcKeyPair 
        

        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcKeyPair(
              string curveName
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Asn1.X9.X9ECParameters ecParam = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName(curveName);

            if (ecParam == null)
                ecParam = Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName(curveName);

            return GenerateEcKeyPair(ecParam, secureRandom);
        } // End Function GenerateEcKeyPair 


    } // End Class KeyGenerator 


} // End Namespace RedmineMailService.CertSSL 
