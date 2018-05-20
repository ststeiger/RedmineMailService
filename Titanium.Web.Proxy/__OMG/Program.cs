
using Titanium.Web.Proxy.Examples.Basic.Helpers;


namespace Titanium.Web.Proxy.Examples.Basic
{


    public class ProxyServerProgram 
    {

        private static readonly ProxyTestController controller = new ProxyTestController();

        public static ProxyTestController Start()
        {
            // fix console hang due to QuickEdit mode
            ConsoleHelper.DisableQuickEditMode();

            // Start proxy controller
            controller.StartProxy();

            // System.Console.WriteLine("Hit any key to exit..");
            // System.Console.WriteLine();
            // System.Console.Read();

            // controller.Stop();

            return controller;
        }


    }


}
