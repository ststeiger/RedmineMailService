
using JetBrains.Annotations;


namespace DasMulli.Win32.ServiceUtils
{


    /// <summary>
    /// The state a service is in
    /// </summary>
    [PublicAPI]
    public enum ServiceState 
        : uint
    {
        /// <summary>
        /// The service is stopped (= not running)
        /// </summary>
        Stopped = 0x00000001,

        /// <summary>
        /// The service is starting
        /// </summary>
        StartPending = 0x00000002,

        /// <summary>
        /// The stopping
        /// </summary>
        StopPending = 0x00000003,

        /// <summary>
        /// The service is running (= started successfully)
        /// </summary>
        Running = 0x00000004,

        /// <summary>
        /// The service is about to resume after being paused
        /// </summary>
        ContinuePending = 0x00000005,

        /// <summary>
        /// The service is about to pause
        /// </summary>
        PausePending = 0x00000006,

        /// <summary>
        /// The service is paused
        /// </summary>
        Paused = 0x00000007,

        /// <summary>
        /// The service is starting.
        /// </summary>
#if NETSTANDARD2_0
        [System.ComponentModel.Browsable(false)]
#endif
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Misspelled, use '" + nameof(StartPending) +"' instead. This member will be removed in upcoming versions.", true)]
        StartPening = StartPending
    }


}
