
namespace RedmineMailService.CertSSL
{


    public class PrivatePublicPemKeyPair
    {
        public string PrivateKey;
        public string PublicKey;


        public void ExportTo(string privateKeyFile, string publicKeyFile)
        {
            System.IO.File.WriteAllText(privateKeyFile, this.PrivateKey, System.Text.Encoding.ASCII);
            System.IO.File.WriteAllText(publicKeyFile, this.PublicKey, System.Text.Encoding.ASCII);
        }

        public static PrivatePublicPemKeyPair ImportFrom(string privateKeyFile, string publicKeyFile)
        {
            PrivatePublicPemKeyPair keyPair = new PrivatePublicPemKeyPair();

            keyPair.PrivateKey = System.IO.File.ReadAllText(privateKeyFile, System.Text.Encoding.ASCII);
            keyPair.PublicKey = System.IO.File.ReadAllText(publicKeyFile, System.Text.Encoding.ASCII);

            return keyPair;
        }


    }


}
