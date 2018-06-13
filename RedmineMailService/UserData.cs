
namespace RedmineMailService.Trash
{
    class UserData
    {
        public static string POP = SecretManager.GetSecret<string>("POP");
        public static string IMAP = SecretManager.GetSecret<string>("IMAP");
        public static string SMTP = SecretManager.GetSecret<string>("SMTP");
        
        public static string Email = SecretManager.GetSecret<string>("Email");
        public static string Password = SecretManager.GetSecret<string>("Password");

        public static string info = SecretManager.GetSecret<string>("info");
        public static string RSN = SecretManager.GetSecret<string>("RSN");
        public static string RSNA = AES.DeCrypt(SecretManager.GetSecret<string>("RSNA"));

        public static string GMailSMTP = SecretManager.GetSecret<string>("GMailSMTP");
        public static string GMail = SecretManager.GetSecret<string>("GMail");
        public static string GMailPassword = AES.DeCrypt(SecretManager.GetSecret<string>("GMailPassword"));
    }
}
