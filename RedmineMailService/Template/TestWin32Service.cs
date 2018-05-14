
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace RedmineMailService
{

    internal class TestWin32Service : DasMulli.Win32.ServiceUtils.IWin32Service
    {
        private readonly string[] commandLineArguments;
        private Microsoft.AspNetCore.Hosting.IWebHost webHost;
        private bool stopRequestedByWindows;


        string DasMulli.Win32.ServiceUtils.IWin32Service.ServiceName
        {
            get
            {
                // throw new System.NotImplementedException();
                return "Test Service";
            }
        }


        public TestWin32Service(string[] commandLineArguments)
        {
            this.commandLineArguments = commandLineArguments;
        } // ENd Constructor 



        void DasMulli.Win32.ServiceUtils.IWin32Service.Start(
            string[] startupArguments
            , DasMulli.Win32.ServiceUtils.ServiceStoppedCallback serviceStoppedCallback)
        {
            // in addition to the arguments that the service has been registered with,
            // each service start may add additional startup parameters.
            // To test this: Open services console, open service details, enter startup arguments and press start.
            string[] combinedArguments;
            if (startupArguments.Length > 0)
            {
                combinedArguments = new string[commandLineArguments.Length + startupArguments.Length];
                System.Array.Copy(commandLineArguments, combinedArguments, commandLineArguments.Length);
                System.Array.Copy(startupArguments, 0, combinedArguments, commandLineArguments.Length, startupArguments.Length);
            }
            else
            {
                combinedArguments = commandLineArguments;
            }


            IConfigurationRoot config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
               .AddCommandLine(combinedArguments) // Microsoft.Extensions.Configuration.CommandLine.dll
               .Build();

            webHost = new Microsoft.AspNetCore.Hosting.WebHostBuilder()
                .UseKestrel() // Microsoft.AspNetCore.Server.Kestrel.dll
                .UseStartup<AspNetCoreStartup>()
                .UseConfiguration(config)
                .Build();


            // Make sure the windows service is stopped if the
            // ASP.NET Core stack stops for any reason
            webHost
               .Services
               .GetRequiredService<IApplicationLifetime>() // Microsoft.Extensions.DependencyInjection
               .ApplicationStopped
               .Register(() =>
               {
                   if (this.stopRequestedByWindows == false)
                   {
                       serviceStoppedCallback();
                   }
               });

            webHost.Start();
        }


        void DasMulli.Win32.ServiceUtils.IWin32Service.Stop()
        {
            stopRequestedByWindows = true;

            if (webHost != null)
                webHost.Dispose();
        }
    }


    internal class AspNetCoreStartup
    {
        public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
        {
            // app.Run(async (context) =>
            Microsoft.AspNetCore.Builder.RunExtensions.Run(app, async (context) =>
            {
                // await context.Response.WriteAsync("Hello World!");
                await Microsoft.AspNetCore.Http.HttpResponseWritingExtensions.WriteAsync(context.Response, "Hello World !");
            });

        }
    }

}


