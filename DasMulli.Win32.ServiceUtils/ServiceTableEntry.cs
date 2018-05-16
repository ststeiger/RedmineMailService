
namespace DasMulli.Win32.ServiceUtils
{

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct ServiceTableEntry
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        internal string serviceName;

        internal System.IntPtr serviceMainFunction;
    }

}
