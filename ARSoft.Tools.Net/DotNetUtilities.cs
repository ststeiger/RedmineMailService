
namespace System
{

    public sealed class DotNetUtilities
    {


        public static Org.BouncyCastle.X509.X509Certificate FromX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate x509Cert)
        {
            return new Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(x509Cert.GetRawCertData());
        }


        // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
        public static Org.BouncyCastle.X509.X509Certificate FromX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Cert)
        {
            // https://stackoverflow.com/questions/8136651/how-can-i-convert-a-bouncycastle-x509certificate-to-an-x509certificate2
            // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
            return new Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(x509Cert.GetRawCertData());
        }


    }

}
