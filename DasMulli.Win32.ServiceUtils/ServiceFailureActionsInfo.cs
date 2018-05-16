
using System.Diagnostics.CodeAnalysis;


namespace DasMulli.Win32.ServiceUtils
{

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty", Justification = "Keep fields to preserve explicit struct layout for marshalling.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal struct ServiceFailureActionsInfo
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] private uint dwResetPeriod;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] private string lpRebootMsg;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] private string lpCommand;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)] private int cActions;
        private System.IntPtr lpsaActions;

        public System.TimeSpan ResetPeriod => System.TimeSpan.FromSeconds(dwResetPeriod);

        public string RebootMsg => lpRebootMsg;

        public string Command => lpCommand;

        public int CountActions => cActions;

        public ScAction[] Actions => lpsaActions.MarshalUnmananagedArrayToStruct<ScAction>(cActions);

        /// <summary>
        /// This is the default, as reported by Windows.
        /// </summary>
        internal static ServiceFailureActionsInfo Default =
            new ServiceFailureActionsInfo {dwResetPeriod = 0, lpRebootMsg = null, lpCommand = null, cActions = 0, lpsaActions = System.IntPtr.Zero};

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFailureActionsInfo"/> class.
        /// </summary>
        internal ServiceFailureActionsInfo(System.TimeSpan resetPeriod, string rebootMessage, string restartCommand
            , System.Collections.Generic.IReadOnlyCollection<ScAction> actions)
        {
            dwResetPeriod = resetPeriod == System.TimeSpan.MaxValue ? uint.MaxValue : (uint)System.Math.Round(resetPeriod.TotalSeconds);
            lpRebootMsg = rebootMessage;
            lpCommand = restartCommand;
            cActions = actions?.Count ?? 0;

            if (null != actions)
            {
                lpsaActions = System.Runtime.InteropServices.Marshal.AllocHGlobal(
                    System.Runtime.InteropServices.Marshal.SizeOf<ScAction>() * cActions
                );

                if (lpsaActions == System.IntPtr.Zero)
                {
                    throw new System.Exception(
                        string.Format("Unable to allocate memory for service action, error was: 0x{0:X}"
                        , System.Runtime.InteropServices.Marshal.GetLastWin32Error()));
                }
                
                System.IntPtr nextAction = lpsaActions;

                foreach (ScAction action in actions)
                {
                    System.Runtime.InteropServices.Marshal.StructureToPtr(action, nextAction, fDeleteOld: false);
                    nextAction = (System.IntPtr) (nextAction.ToInt64() + System.Runtime.InteropServices.Marshal.SizeOf<ScAction>());
                }
            }
            else
            {
                lpsaActions = System.IntPtr.Zero;
            }
        }
    }
}