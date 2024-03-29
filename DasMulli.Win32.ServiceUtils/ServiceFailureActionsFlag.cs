﻿
namespace DasMulli.Win32.ServiceUtils
{


    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct ServiceFailureActionsFlag
    {
        private bool _fFailureActionsOnNonCrashFailures;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFailureActionsFlag"/> struct.
        /// </summary>
        public ServiceFailureActionsFlag(bool enabled)
        {
            _fFailureActionsOnNonCrashFailures = enabled;
        }

        public bool Flag
        {
            get => _fFailureActionsOnNonCrashFailures;
            set => _fFailureActionsOnNonCrashFailures = value;
        }

    }


}
