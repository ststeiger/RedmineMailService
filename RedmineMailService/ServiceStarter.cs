
using System;
using System.Linq;


namespace RedmineMailService
{


    public static class ServiceStarter
    {
        private const string RunAsServiceFlag = "--run-as-service";
        private const string RegisterServiceFlag = "--register-service";
        private const string UnregisterServiceFlag = "--unregister-service";
        private const string InteractiveFlag = "--interactive";
        private const string StopServiceFlag = "--stop-service";
        private const string RestartServiceFlag = "--restart-service";
        
        
        // systemctl status mssql-server
        // initctl show-config <servicename> t
        // systemctl list-unit-files --type=service
        
        // initctl list
        // service --status-all
        
        // sudo systemctl start mssql-server
        // /etc/init.d/mysql start
        
        // sudo systemctl stop mssql-server
        // /etc/init.d/mysql stop
        
        // sudo systemctl restart mssql-server
        // /etc/init.d/mysql restart
        
        // sudo systemctl enable mssql-server
        // sudo update-rc.d mysql defaults
        
        // sudo systemctl disable mssql-server
        // sudo update-rc.d -f mysql remove
        
        private const string ServiceName = "DemoService";
        private const string ServiceDisplayName = "Demo .NET Core Service";
        private const string ServiceDescription = "Demo ASP.NET Core Service running on .NET Core";


        public static void ipc()
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication
            System.IO.Pipes.NamedPipeServerStream pipeServer = 
                new System.IO.Pipes.NamedPipeServerStream("testpipe"
                    , System.IO.Pipes.PipeDirection.InOut, 1);
            
            // Wait for a client to connect
            pipeServer.WaitForConnection();
            
            // pipeServer.Write(
            System.IO.MemoryMappedFiles.MemoryMappedFile.CreateOrOpen("lul", 2);
            
            // System.IO.MemoryMappedFiles
        }
        
        
        public static void Start(string[] args)
        {
            // args = new string[] { "--interactive" };
            args = new string[] { "--run-as-service" };
            
            // Demo ASP.NET Core Service running on.NET Core
            // This demo application is intened to be run as windows service.Use one of the following options:
            //
            // --register-service       Registers and starts this program as a windows service named "Demo .NET Core Service"
            //                          All additional arguments will be passed to ASP.NET Core's WebHostBuilder.
            // --unregister-service     Removes the windows service creatd by --register - service.  
            // --interactive            Runs the underlying asp.net core app.Useful to test arguments.

            try
            {
                if (System.Linq.Enumerable.Contains(args, RunAsServiceFlag))
                {
                    RunAsService(args);
                }
                else if (System.Linq.Enumerable.Contains(args, RegisterServiceFlag))
                {
                    RegisterService();
                }
                else if (System.Linq.Enumerable.Contains(args, UnregisterServiceFlag))
                {
                    UnregisterService();
                }
                else if (System.Linq.Enumerable.Contains(args, InteractiveFlag))
                {
                    RunInteractive(args);
                }
                else
                {
                    DisplayHelp();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"An error ocurred: {ex.Message}");
            }
        }


        private static void RunAsService(string[] args)
        {
            DasMulli.Win32.ServiceUtils.IWin32Service service = 
                new TestWin32Service(args.Where(a => a != RunAsServiceFlag).ToArray());
            
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                service.Start(new string[0], () => { });
            }
            else
            {
                DasMulli.Win32.ServiceUtils.Win32ServiceHost serviceHost =
                    new DasMulli.Win32.ServiceUtils.Win32ServiceHost(service);
                serviceHost.Run();
            }
        }


        private static void RunInteractive(string[] args)
        {
            DasMulli.Win32.ServiceUtils.IWin32Service service =
                new TestWin32Service(args.Where(a => a != InteractiveFlag).ToArray());

            service.Start(new string[0], () => { });
            System.Console.WriteLine("Running interactively, press enter to stop.");
            System.Console.ReadLine();
            service.Stop();
        }
        
        
        private static void RegisterService()
        {
            // Environment.GetCommandLineArgs() includes the current DLL from a "dotnet my.dll --register-service" call, which is not passed to Main()
            string[] remainingArgs = System.Environment.GetCommandLineArgs()
                .Where(arg => arg != RegisterServiceFlag)
                .Select(EscapeCommandLineArgument)
                .Append(RunAsServiceFlag)
                .ToArray();

            string host = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            if (!host.EndsWith("dotnet.exe", System.StringComparison.OrdinalIgnoreCase))
            {
                // For self-contained apps, skip the dll path
                remainingArgs = remainingArgs.Skip(1).ToArray();
            }

            string fullServiceCommand = host + " " + string.Join(" ", remainingArgs);

            // Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
            // Note that when the service is already registered and running, it will be reconfigured but not restarted
            DasMulli.Win32.ServiceUtils.ServiceDefinition serviceDefinition =
                new DasMulli.Win32.ServiceUtils.ServiceDefinitionBuilder(ServiceName)
                .WithDisplayName(ServiceDisplayName)
                .WithDescription(ServiceDescription)
                .WithBinaryPath(fullServiceCommand)
                .WithCredentials(DasMulli.Win32.ServiceUtils.Win32ServiceCredentials.LocalSystem)
                .WithAutoStart(true)
                .Build();

            new DasMulli.Win32.ServiceUtils.Win32ServiceManager().CreateOrUpdateService(serviceDefinition, startImmediately: true);

            System.Console.WriteLine($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }


        private static void UnregisterService()
        {
            new DasMulli.Win32.ServiceUtils.Win32ServiceManager()
                .DeleteService(ServiceName);

            System.Console.WriteLine($@"Successfully unregistered service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }


        private static void DisplayHelp()
        {
            System.Console.WriteLine(ServiceDescription);
            System.Console.WriteLine();
            System.Console.WriteLine("This demo application is intened to be run as windows service. Use one of the following options:");
            System.Console.WriteLine("  --register-service        Registers and starts this program as a windows service named \"" + ServiceDisplayName + "\"");
            System.Console.WriteLine("                            All additional arguments will be passed to ASP.NET Core's WebHostBuilder.");
            System.Console.WriteLine("  --unregister-service      Removes the windows service creatd by --register-service.");
            System.Console.WriteLine("  --interactive             Runs the underlying asp.net core app. Useful to test arguments.");
        }


        private static string EscapeCommandLineArgument(string arg)
        {
            // https://stackoverflow.com/questions/5510343/escape-command-line-arguments-in-c-sharp/6040946#6040946
            // http://stackoverflow.com/a/6040946/784387
            arg = System.Text.RegularExpressions.Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
            arg = "\"" + System.Text.RegularExpressions.Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
            return arg;
        }


    }
}
