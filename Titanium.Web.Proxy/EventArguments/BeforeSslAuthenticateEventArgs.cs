﻿using System;
using System.Threading;

namespace Titanium.Web.Proxy.EventArguments
{
    /// <summary>
    ///     This is used in transparent endpoint before authenticating client.
    /// </summary>
    public class BeforeSslAuthenticateEventArgs : EventArgs
    {
        internal readonly CancellationTokenSource TaskCancellationSource;

        internal BeforeSslAuthenticateEventArgs(CancellationTokenSource taskCancellationSource)
        {
            TaskCancellationSource = taskCancellationSource;
        }

        /// <summary>
        ///     The server name indication hostname if available. Otherwise the generic certificate hostname of
        ///     TransparentEndPoint.
        /// </summary>
        public string SniHostName { get; internal set; }

        /// <summary>
        ///     Should we decrypt the SSL request?
        ///     If true we decrypt with fake certificate.
        ///     If false we relay the connection to the hostname mentioned in SniHostname.
        /// </summary>
        public bool DecryptSsl { get; set; } = true;

        /// <summary>
        ///     Terminate the request abruptly by closing client/server connections.
        /// </summary>
        public void TerminateSession()
        {
            TaskCancellationSource.Cancel();
        }
    }
}
