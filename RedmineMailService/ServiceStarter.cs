
namespace RedmineMailService
{


    public static class ServiceStarter
    {
        private const string RUN_AS_SERVICE = "--run-as-service"; // Append
        private const string REGISTER_SERVICE = "--register-service"; // X_
        private const string UNREGISTER_SERVICE = "--unregister-service"; // OK
        private const string INTERACTIVE = "--interactive"; // OK

        private const string STOP_SERVICE = "--stop-service";
        private const string RESTART_SERVICE = "--restart-service";
        
        
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
            // args = new string[] { INTERACTIVE };
            args = new string[] { REGISTER_SERVICE };
            // args = new string[] { RUN_AS_SERVICE };
            // args = new string[] { UNREGISTER_SERVICE };

            // Demo ASP.NET Core Service running on.NET Core
            // This demo application is intened to be run as windows service.Use one of the following options:
            //
            // --register-service       Registers and starts this program as a windows service named "Demo .NET Core Service"
            //                          All additional arguments will be passed to ASP.NET Core's WebHostBuilder.
            // --unregister-service     Removes the windows service creatd by --register - service.  
            // --interactive            Runs the underlying asp.net core app. Useful to test arguments.

            try
            {
                if(-1 != System.Array.FindIndex(args, x => string.Equals(x, RUN_AS_SERVICE, System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    RunAsService(args);
                }
                else if (-1 != System.Array.FindIndex(args, x => string.Equals(x, REGISTER_SERVICE, System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    RegisterService();
                }
                else if (-1 != System.Array.FindIndex(args, x => string.Equals(x, UNREGISTER_SERVICE, System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    UnregisterService();
                }
                else if (-1 != System.Array.FindIndex(args, x => string.Equals(x, INTERACTIVE, System.StringComparison.InvariantCultureIgnoreCase)))
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
                new TestWin32Service(RemoveFlag(args, RUN_AS_SERVICE));

            
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                DasMulli.Win32.ServiceUtils.Win32ServiceHost serviceHost =
                    new DasMulli.Win32.ServiceUtils.Win32ServiceHost(service);
                serviceHost.Run();
            }
            else if (System.Environment.OSVersion.Platform == System.PlatformID.Unix) // includes Mac 
            {
                service.Start(new string[0], () => { });
            }
            else
            {
                System.Console.WriteLine("Unknown operating system.");
                System.Console.WriteLine("Attempting to start service.");
                service.Start(new string[0], () => { }); // ?
            }

        }


        private static T[] RemoveIndices<T>(T[] indicesArray, int removeAt)
        {
            if (indicesArray == null || indicesArray.Length == 0)
                return indicesArray;

            T[] newIndicesArray = new T[indicesArray.Length - 1];

            int i = 0;
            int j = 0;
            while (i < indicesArray.Length)
            {
                if (i != removeAt)
                {
                    newIndicesArray[j] = indicesArray[i];
                    j++;
                } // End if (i != removeAt) 

                i++;
            } // Whend

            return newIndicesArray;
        } // End Function RemoveIndices 


        private static string[] RemoveFlag(string[] args, string argument)
        {
            if (args == null)
                return null;

            int index = System.Array.FindIndex(args, x => string.Equals(x, argument, System.StringComparison.InvariantCultureIgnoreCase));
            if (index != -1)
            {
                return RemoveIndices(args, index);
            } // End if (index != -1) 

            return args;
        } // End Function RemoveFlag 


        private static void RunInteractive(string[] args)
        {
            DasMulli.Win32.ServiceUtils.IWin32Service service =
                new TestWin32Service(RemoveFlag(args, INTERACTIVE));

            service.Start(new string[0], () => { });
            System.Console.WriteLine("Running interactively, press enter to stop.");
            System.Console.ReadLine();
            service.Stop();
        }
        
        
        private static void RegisterService()
        {

            // Environment.GetCommandLineArgs() includes the current DLL from a "dotnet my.dll --register-service" call, which is not passed to Main()
            string[] remainingArgs = RemoveFlag(System.Environment.GetCommandLineArgs(), REGISTER_SERVICE);

            for (int i = 0; i < remainingArgs.Length; ++i)
            {
                remainingArgs[i] = EscapeCommandLineArgument(remainingArgs[i]);
            }

            string[] args = new string[remainingArgs.Length + 1];
            System.Array.Copy(remainingArgs, args, remainingArgs.Length);
            args[args.Length - 1] = RUN_AS_SERVICE;


            string host = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            if (!host.EndsWith("dotnet.exe", System.StringComparison.OrdinalIgnoreCase))
            {
                // For self-contained apps, skip the dll path
                args = RemoveIndices(args, 0);
            }

            string fullServiceCommand = host + " " + string.Join(" ", args);

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

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                string serviceFileContent = @"
[Unit]
Description=Redmine Mail Service - tickets from e-mail

[Service]
WorkingDirectory=/web/services/RedmineMailService
ExecStart=/usr/bin/dotnet /web/services/RedmineMailService/RedmineMailService.dll
Restart=always
RestartSec=10  # Restart service after 10 seconds if dotnet service crashes
SyslogIdentifier=RedmineMailService
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=DOTNET_CLI_TELEMETRY_OPTOUT=1

[Install]
WantedBy=multi-user.target

";

                string serviceFile = @"/etc/systemd/system/RedmineMailService.service";
                if (!System.IO.File.Exists(serviceFile))
                    System.IO.File.WriteAllText(serviceFile, serviceFileContent, System.Text.Encoding.UTF8);
                
                Posix.passwd_t pw = Posix.Syscalls.getpwnam("www-data");
                Posix.Syscalls.chown(serviceFile, pw.pw_uid, pw.pw_gid);
                
                // Remove dependency on mono...
                // Mono.Unix.Native.Passwd pw = Mono.Unix.Native.Syscall.getpwnam("www-data");
                // Mono.Unix.Native.Syscall.chown(serviceFile, pw.pw_uid, pw.pw_gid);
                
                using (System.Diagnostics.Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo("sudo", "systemctl enable RedmineMailService")
                })
                {
                    p.Start();
                    p.WaitForExit();
                }
                
                using (System.Diagnostics.Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo("sudo", "systemctl restart RedmineMailService")
                })
                {
                    p.Start();
                    p.WaitForExit();
                }
                
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                new DasMulli.Win32.ServiceUtils.Win32ServiceManager().CreateOrUpdateService(serviceDefinition, startImmediately: true);
            }
            else
            {
                throw new System.PlatformNotSupportedException("RegisterService does not support your OS.", new System.NotImplementedException("RegisterService for non-Linux and non-Windows platforms not implemented."));
            }
            
            System.Console.WriteLine($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }


        private static void UnregisterService()
        {

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {

                using (System.Diagnostics.Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo("sudo", "systemctl stop RedmineMailService")
                })
                {
                    p.Start();
                    p.WaitForExit();
                }

                using (System.Diagnostics.Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo("sudo", "systemctl disable RedmineMailService")
                })
                {
                    p.Start();
                    p.WaitForExit();
                }
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                new DasMulli.Win32.ServiceUtils.Win32ServiceManager()
                .DeleteService(ServiceName);
            }
            else
            {
                throw new System.PlatformNotSupportedException("UnregisterService does not support your OS.", new System.NotImplementedException("UnregisterService for non-Linux and non-Windows platforms not implemented."));
            }

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
