
using System.Runtime.InteropServices;


namespace DasMulli.Win32.ServiceUtils
{
    [JetBrains.Annotations.UsedImplicitly(JetBrains.Annotations.ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Subclassed by test proxy")]
    internal class ServiceHandle 
        : System.Runtime.InteropServices.SafeHandle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Exposed for testing via InternalsVisibleTo.")]
        internal INativeInterop NativeInterop { get; set; } = Win32Interop.Wrapper;

        internal ServiceHandle()
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

        public virtual void Start(bool throwIfAlreadyRunning = true)
        {
            if (!NativeInterop.StartServiceW(this, 0, System.IntPtr.Zero))
            {
                System.Int32 win32Error = Marshal.GetLastWin32Error();
                if (win32Error != KnownWin32ErrorCoes.ERROR_SERVICE_ALREADY_RUNNING || throwIfAlreadyRunning)
                {
                    throw new System.ComponentModel.Win32Exception(win32Error);
                }
            }
        }

        public virtual void Delete()
        {
            if (!NativeInterop.DeleteService(this))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public virtual void SetDescription(string description)
        {
            ServiceDescriptionInfo descriptionInfo = new ServiceDescriptionInfo(description ?? string.Empty);
            System.IntPtr lpDescriptionInfo = Marshal.AllocHGlobal(Marshal.SizeOf<ServiceDescriptionInfo>());
            try
            {
                Marshal.StructureToPtr(descriptionInfo, lpDescriptionInfo, fDeleteOld: false);
                try
                {
                    if (!NativeInterop.ChangeServiceConfig2W(this, ServiceConfigInfoTypeLevel.ServiceDescription, lpDescriptionInfo))
                    {
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    Marshal.DestroyStructure<ServiceDescriptionInfo>(lpDescriptionInfo);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(lpDescriptionInfo);
            }
        }

        public virtual void SetFailureActions(ServiceFailureActions serviceFailureActions)
        {
            ServiceFailureActionsInfo failureActions = serviceFailureActions == null ? ServiceFailureActionsInfo.Default 
                : new ServiceFailureActionsInfo(serviceFailureActions.ResetPeriod
                , serviceFailureActions.RebootMessage
                , serviceFailureActions.RestartCommand
                , serviceFailureActions.Actions
            );

            System.IntPtr lpFailureActions = Marshal.AllocHGlobal(Marshal.SizeOf<ServiceFailureActionsInfo>());
            try
            {
                Marshal.StructureToPtr(failureActions, lpFailureActions, fDeleteOld: false);
                try
                {
                    if (!NativeInterop.ChangeServiceConfig2W(this, ServiceConfigInfoTypeLevel.FailureActions, lpFailureActions))
                    {
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    Marshal.DestroyStructure<ServiceFailureActionsInfo>(lpFailureActions);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(lpFailureActions);
            }
        }

        public virtual void SetFailureActionFlag(bool enabled)
        {
            ServiceFailureActionsFlag failureActionsFlag = new ServiceFailureActionsFlag(enabled);
            System.IntPtr lpFailureActionsFlag = Marshal.AllocHGlobal(Marshal.SizeOf<ServiceFailureActionsFlag>());
            try
            {
                Marshal.StructureToPtr(failureActionsFlag, lpFailureActionsFlag, fDeleteOld: false);
                try
                {
                    bool result = NativeInterop.ChangeServiceConfig2W(this, ServiceConfigInfoTypeLevel.FailureActionsFlag, lpFailureActionsFlag);
                    if (!result)
                    {
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    Marshal.DestroyStructure<ServiceFailureActionsFlag>(lpFailureActionsFlag);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(lpFailureActionsFlag);
            }
        }

        public virtual void ChangeConfig(string displayName, string binaryPath, ServiceType serviceType, ServiceStartType startupType, ErrorSeverity errorSeverity, Win32ServiceCredentials credentials)
        {
            bool success = NativeInterop.ChangeServiceConfigW(this
                , serviceType, startupType, errorSeverity
                , binaryPath, null, System.IntPtr.Zero, null
                , credentials.UserName, credentials.Password, displayName);

            if (!success)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public virtual unsafe void SetDelayedAutoStartFlag(bool delayedAutoStart)
        {
            int value = delayedAutoStart ? 1 : 0;
            bool success = NativeInterop.ChangeServiceConfig2W(this
                , ServiceConfigInfoTypeLevel.DelayedAutoStartInfo
                , new System.IntPtr(&value)
            );

            if (!success)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}