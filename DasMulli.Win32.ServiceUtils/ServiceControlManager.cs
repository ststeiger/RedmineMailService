
using JetBrains.Annotations;


namespace DasMulli.Win32.ServiceUtils
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    internal class ServiceControlManager : System.Runtime.InteropServices.SafeHandle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global"
            , Justification = "Exposed for testing via InternalsVisibleTo.")]
        internal INativeInterop NativeInterop { get; set; } = Win32Interop.Wrapper;

        internal ServiceControlManager() 
            : base(System.IntPtr.Zero, ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeInterop.CloseServiceHandle(handle);
        }

        public override bool IsInvalid
        {
            [System.Security.SecurityCritical]
            get
            {
                return handle == System.IntPtr.Zero;
            }
        }

        internal static ServiceControlManager Connect(INativeInterop nativeInterop, string machineName, string databaseName, ServiceControlManagerAccessRights desiredAccessRights)
        {
            ServiceControlManager mgr = nativeInterop.OpenSCManagerW(machineName, databaseName, desiredAccessRights);

            mgr.NativeInterop = nativeInterop;

            if (mgr.IsInvalid)
            {

                throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            }

            return mgr;
        }

        public ServiceHandle CreateService(string serviceName
            , string displayName
            , string binaryPath
            , ServiceType serviceType
            , ServiceStartType startupType
            , ErrorSeverity errorSeverity
            , Win32ServiceCredentials credentials)
        {
            ServiceHandle service = NativeInterop.CreateServiceW(this, serviceName, displayName, ServiceControlAccessRights.All, serviceType, startupType, errorSeverity,
                binaryPath, null,
                System.IntPtr.Zero, null, credentials.UserName, credentials.Password);

            service.NativeInterop = NativeInterop;

            if (service.IsInvalid)
            {
                throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            }
            
            return service;
        }

        public ServiceHandle OpenService(string serviceName, ServiceControlAccessRights desiredControlAccess)
        {
            ServiceHandle service;
            System.ComponentModel.Win32Exception errorException;

            if (!TryOpenService(serviceName, desiredControlAccess, out service, out errorException))
            {
                throw errorException;
            }

            return service;
        }

        public virtual bool TryOpenService(string serviceName
            , ServiceControlAccessRights desiredControlAccess
            , out ServiceHandle serviceHandle
            , out System.ComponentModel.Win32Exception errorException)
        {
            ServiceHandle service = NativeInterop.OpenServiceW(this, serviceName, desiredControlAccess);

            service.NativeInterop = NativeInterop;

            if (service.IsInvalid)
            {
                errorException = new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                serviceHandle = null;
                return false;
            }

            serviceHandle = service;
            errorException = null;
            return true;
        }
    }
}