
namespace RedmineMailService
{


    class Program
    {


        [System.STAThread]
        static void Main(string[] args)
        {
            // https://github.com/dasMulli/dotnet-win32-service
            ServiceStarter.Start(args);

            // RedmineMailService.Trash.MailSender.Test();

            RedmineMailService.TestMailReader.Test();

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace RedmineMailService 
