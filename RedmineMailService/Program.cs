
namespace RedmineMailService
{
    class Program
    {
        static void Main(string[] args)
        {
            // https://github.com/dasMulli/dotnet-win32-service
            ServiceStarter.Start(args);

            System.Console.WriteLine("Hello World!");
        }
    }
}
