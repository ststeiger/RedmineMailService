
namespace RedmineMailService.CertSSL
{


    public class CertificateUtil
    {


        // https://stackoverflow.com/questions/13806299/how-to-create-a-self-signed-certificate-using-c
        static void MakeCert()
        {

            using ( System.Security.Cryptography.ECDsa ecdsa = System.Security.Cryptography.ECDsa.Create()) // generate asymmetric key pair
            {
                System.Security.Cryptography.X509Certificates.CertificateRequest req = 
                    new System.Security.Cryptography.X509Certificates.CertificateRequest("cn=foobar", ecdsa
                    , System.Security.Cryptography.HashAlgorithmName.SHA512
                );

                using (System.Security.Cryptography.X509Certificates.X509Certificate2 cert =
                    req.CreateSelfSigned(System.DateTimeOffset.Now, System.DateTimeOffset.Now.AddYears(5)
                ))
                {
                    // Create PFX (PKCS #12) with private key
                    System.IO.File.WriteAllBytes("d:\\mycert.pfx", cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pfx));

                    // Create Base 64 encoded CER (public key only)
                    System.IO.File.WriteAllText("d:\\mycert.cer",
                        "-----BEGIN CERTIFICATE-----\r\n"
                        + System.Convert.ToBase64String(
                              cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert)
                            , System.Base64FormattingOptions.InsertLineBreaks
                        )
                        + "\r\n-----END CERTIFICATE-----");
                } // End Using cert 

            } // End Using ecdsa 
      
        } // End Sub MakeCert 


    } // End Class 

} // End Namespace 
