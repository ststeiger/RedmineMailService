
namespace DasMulli.Win32.ServiceUtils
{
    internal delegate void ServiceControlHandler(ServiceControlCommand control
        , uint eventType
        , System.IntPtr eventData
        , System.IntPtr eventContext);
}
