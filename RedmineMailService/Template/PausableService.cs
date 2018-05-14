
namespace RedmineMailService
{

    // Skeleton
    internal class PausableService : DasMulli.Win32.ServiceUtils.IPausableWin32Service

    {
        string DasMulli.Win32.ServiceUtils.IWin32Service.ServiceName => throw new System.NotImplementedException();


        public PausableService()
        { } // ENd Constructor 


        void DasMulli.Win32.ServiceUtils.IPausableWin32Service.Continue()
        {
            throw new System.NotImplementedException();
        }

        void DasMulli.Win32.ServiceUtils.IPausableWin32Service.Pause()
        {
            throw new System.NotImplementedException();
        }

        void DasMulli.Win32.ServiceUtils.IWin32Service.Start(
            string[] startupArguments,
            DasMulli.Win32.ServiceUtils.ServiceStoppedCallback serviceStoppedCallback)
        {
            throw new System.NotImplementedException();
        }

        void DasMulli.Win32.ServiceUtils.IWin32Service.Stop()
        {
            throw new System.NotImplementedException();
        }
    }



}
