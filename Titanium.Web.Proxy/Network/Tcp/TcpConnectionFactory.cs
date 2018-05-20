﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StreamExtended.Network;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Extensions;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace Titanium.Web.Proxy.Network.Tcp
{
    /// <summary>
    ///     A class that manages Tcp Connection to server used by this proxy server.
    /// </summary>
    internal class TcpConnectionFactory : IDisposable
    {
        //Tcp server connection pool cache
        private readonly ConcurrentDictionary<string, ConcurrentQueue<TcpServerConnection>> cache
            = new ConcurrentDictionary<string, ConcurrentQueue<TcpServerConnection>>();

        //Tcp connections waiting to be disposed by cleanup task
        private readonly ConcurrentBag<TcpServerConnection> disposalBag =
            new ConcurrentBag<TcpServerConnection>();

        //cache object race operations lock
        private readonly SemaphoreSlim @lock = new SemaphoreSlim(1);

        private volatile bool runCleanUpTask = true;

        internal TcpConnectionFactory(ProxyServer server)
        {
            this.server = server;
            Task.Run(async () => await clearOutdatedConnections());
        }

        internal ProxyServer server { get; set; }

        internal string GetConnectionCacheKey(string remoteHostName, int remotePort,
            bool isHttps, List<SslApplicationProtocol> applicationProtocols,
            ProxyServer proxyServer, IPEndPoint upStreamEndPoint, ExternalProxy externalProxy)
        {
            //http version is ignored since its an application level decision b/w HTTP 1.0/1.1
            //also when doing connect request MS Edge browser sends http 1.0 but uses 1.1 after server sends 1.1 its response.
            //That can create cache miss for same server connection unneccessarily expecially when prefetcing with Connect.
            //http version 2 is separated using applicationProtocols below.
            var cacheKeyBuilder = new StringBuilder($"{remoteHostName}-{remotePort}-" +
                                                  //when creating Tcp client isConnect won't matter
                                                  $"{isHttps}-");
            if (applicationProtocols != null)
            {
                foreach (var protocol in applicationProtocols.OrderBy(x => x))
                {
                    cacheKeyBuilder.Append($"{protocol}-");
                }
            }

            cacheKeyBuilder.Append(upStreamEndPoint != null
                ? $"{upStreamEndPoint.Address}-{upStreamEndPoint.Port}-"
                : string.Empty);
            cacheKeyBuilder.Append(externalProxy != null ? $"{externalProxy.GetCacheKey()}-" : string.Empty);

            return cacheKeyBuilder.ToString();

        }

        /// <summary>
        ///     Gets the connection cache key.
        /// </summary>
        /// <param name="args">The session event arguments.</param>
        /// <param name="applicationProtocol"></param>
        /// <returns></returns>
        internal async Task<string> GetConnectionCacheKey(ProxyServer server, SessionEventArgsBase args,
            SslApplicationProtocol applicationProtocol)
        {
            List<SslApplicationProtocol> applicationProtocols = null;
            if (applicationProtocol != default(SslApplicationProtocol))
            {
                applicationProtocols = new List<SslApplicationProtocol> { applicationProtocol };
            }

            ExternalProxy customUpStreamProxy = null;

            bool isHttps = args.IsHttps;
            if (server.GetCustomUpStreamProxyFunc != null)
            {
                customUpStreamProxy = await server.GetCustomUpStreamProxyFunc(args);
            }

            args.CustomUpStreamProxyUsed = customUpStreamProxy;

            return GetConnectionCacheKey(
                args.WebSession.Request.RequestUri.Host,
                args.WebSession.Request.RequestUri.Port,
                isHttps, applicationProtocols,
                server, args.WebSession.UpStreamEndPoint ?? server.UpStreamEndPoint,
                customUpStreamProxy ?? (isHttps ? server.UpStreamHttpsProxy : server.UpStreamHttpProxy));
        }


        /// <summary>
        ///     Create a server connection.
        /// </summary>
        /// <param name="args">The session event arguments.</param>
        /// <param name="isConnect">Is this a CONNECT request.</param>
        /// <param name="applicationProtocol"></param>
        /// <param name="cancellationToken">The cancellation token for this async task.</param>
        /// <returns></returns>
        internal Task<TcpServerConnection> GetServerConnection(ProxyServer server, SessionEventArgsBase args, bool isConnect,
            SslApplicationProtocol applicationProtocol, bool noCache, CancellationToken cancellationToken)
        {
            List<SslApplicationProtocol> applicationProtocols = null;
            if (applicationProtocol != default(SslApplicationProtocol))
            {
                applicationProtocols = new List<SslApplicationProtocol> { applicationProtocol };
            }

            return GetServerConnection(server, args, isConnect, applicationProtocols, noCache, cancellationToken);
        }

        /// <summary>
        ///     Create a server connection.
        /// </summary>
        /// <param name="args">The session event arguments.</param>
        /// <param name="isConnect">Is this a CONNECT request.</param>
        /// <param name="applicationProtocols"></param>
        /// <param name="cancellationToken">The cancellation token for this async task.</param>
        /// <returns></returns>
        internal async Task<TcpServerConnection> GetServerConnection(ProxyServer server, SessionEventArgsBase args, bool isConnect,
            List<SslApplicationProtocol> applicationProtocols, bool noCache, CancellationToken cancellationToken)
        {
            ExternalProxy customUpStreamProxy = null;

            bool isHttps = args.IsHttps;
            if (server.GetCustomUpStreamProxyFunc != null)
            {
                customUpStreamProxy = await server.GetCustomUpStreamProxyFunc(args);
            }

            args.CustomUpStreamProxyUsed = customUpStreamProxy;

            return await GetServerConnection(
                args.WebSession.Request.RequestUri.Host,
                args.WebSession.Request.RequestUri.Port,
                args.WebSession.Request.HttpVersion,
                isHttps, applicationProtocols, isConnect,
                server, args.WebSession.UpStreamEndPoint ?? server.UpStreamEndPoint,
                customUpStreamProxy ?? (isHttps ? server.UpStreamHttpsProxy : server.UpStreamHttpProxy),
                noCache, cancellationToken);
        }
        /// <summary>
        ///     Gets a TCP connection to server from connection pool.
        /// </summary>
        /// <param name="remoteHostName">The remote hostname.</param>
        /// <param name="remotePort">The remote port.</param>
        /// <param name="httpVersion">The http version to use.</param>
        /// <param name="isHttps">Is this a HTTPS request.</param>
        /// <param name="applicationProtocols">The list of HTTPS application level protocol to negotiate if needed.</param>
        /// <param name="isConnect">Is this a CONNECT request.</param>
        /// <param name="proxyServer">The current ProxyServer instance.</param>
        /// <param name="upStreamEndPoint">The local upstream endpoint to make request via.</param>
        /// <param name="externalProxy">The external proxy to make request via.</param>
        /// <param name="noCache">Not from cache/create new connection.</param>
        /// <param name="cancellationToken">The cancellation token for this async task.</param>
        /// <returns></returns>
        internal async Task<TcpServerConnection> GetServerConnection(string remoteHostName, int remotePort,
            Version httpVersion, bool isHttps, List<SslApplicationProtocol> applicationProtocols, bool isConnect,
            ProxyServer proxyServer, IPEndPoint upStreamEndPoint, ExternalProxy externalProxy,
            bool noCache, CancellationToken cancellationToken)
        {
            var cacheKey = GetConnectionCacheKey(remoteHostName, remotePort,
                isHttps, applicationProtocols,
                proxyServer, upStreamEndPoint, externalProxy);

            if (proxyServer.EnableConnectionPool && !noCache)
            {
                if (cache.TryGetValue(cacheKey, out var existingConnections))
                {
                    while (existingConnections.Count > 0)
                    {
                        if (existingConnections.TryDequeue(out var recentConnection))
                        {
                            //+3 seconds for potential delay after getting connection
                            var cutOff = DateTime.Now.AddSeconds(-1 * proxyServer.ConnectionTimeOutSeconds + 3);

                            if (recentConnection.LastAccess > cutOff
                                && isGoodConnection(recentConnection.TcpClient))
                            {
                                return recentConnection;
                            }

                            disposalBag.Add(recentConnection);
                        }
                    }
                }
            }

            var connection = await createServerConnection(remoteHostName, remotePort, httpVersion, isHttps,
                applicationProtocols, isConnect, proxyServer, upStreamEndPoint, externalProxy, cancellationToken);

            connection.CacheKey = cacheKey;

            return connection;
        }

        /// <summary>
        ///     Creates a TCP connection to server
        /// </summary>
        /// <param name="remoteHostName">The remote hostname.</param>
        /// <param name="remotePort">The remote port.</param>
        /// <param name="httpVersion">The http version to use.</param>
        /// <param name="isHttps">Is this a HTTPS request.</param>
        /// <param name="applicationProtocols">The list of HTTPS application level protocol to negotiate if needed.</param>
        /// <param name="isConnect">Is this a CONNECT request.</param>
        /// <param name="proxyServer">The current ProxyServer instance.</param>
        /// <param name="upStreamEndPoint">The local upstream endpoint to make request via.</param>
        /// <param name="externalProxy">The external proxy to make request via.</param>
        /// <param name="cancellationToken">The cancellation token for this async task.</param>
        /// <returns></returns>
        private async Task<TcpServerConnection> createServerConnection(string remoteHostName, int remotePort,
            Version httpVersion, bool isHttps, List<SslApplicationProtocol> applicationProtocols, bool isConnect,
            ProxyServer proxyServer, IPEndPoint upStreamEndPoint, ExternalProxy externalProxy,
            CancellationToken cancellationToken)
        {
            bool useUpstreamProxy = false;

            // check if external proxy is set for HTTP/HTTPS
            if (externalProxy != null &&
                !(externalProxy.HostName == remoteHostName && externalProxy.Port == remotePort))
            {
                useUpstreamProxy = true;

                // check if we need to ByPass
                if (externalProxy.BypassLocalhost && NetworkHelper.IsLocalIpAddress(remoteHostName))
                {
                    useUpstreamProxy = false;
                }
            }

            TcpClient tcpClient = null;
            CustomBufferedStream stream = null;

            SslApplicationProtocol negotiatedApplicationProtocol = default(SslApplicationProtocol);

            try
            {
                tcpClient = new TcpClient(upStreamEndPoint)
                {
                    ReceiveTimeout = proxyServer.ConnectionTimeOutSeconds * 1000,
                    SendTimeout = proxyServer.ConnectionTimeOutSeconds * 1000,
                    SendBufferSize = proxyServer.BufferSize,
                    ReceiveBufferSize = proxyServer.BufferSize
                };

                await proxyServer.InvokeConnectionCreateEvent(tcpClient, false);

                // If this proxy uses another external proxy then create a tunnel request for HTTP/HTTPS connections
                if (useUpstreamProxy)
                {
                    await tcpClient.ConnectAsync(externalProxy.HostName, externalProxy.Port);
                }
                else
                {
                    await tcpClient.ConnectAsync(remoteHostName, remotePort);
                }

                stream = new CustomBufferedStream(tcpClient.GetStream(), proxyServer.BufferPool, proxyServer.BufferSize);

                if (useUpstreamProxy && (isConnect || isHttps))
                {
                    var writer = new HttpRequestWriter(stream, proxyServer.BufferPool, proxyServer.BufferSize);
                    var connectRequest = new ConnectRequest
                    {
                        OriginalUrl = $"{remoteHostName}:{remotePort}",
                        HttpVersion = httpVersion
                    };

                    connectRequest.Headers.AddHeader(KnownHeaders.Connection, KnownHeaders.ConnectionKeepAlive);

                    if (!string.IsNullOrEmpty(externalProxy.UserName) && externalProxy.Password != null)
                    {
                        connectRequest.Headers.AddHeader(HttpHeader.ProxyConnectionKeepAlive);
                        connectRequest.Headers.AddHeader(
                            HttpHeader.GetProxyAuthorizationHeader(externalProxy.UserName, externalProxy.Password));
                    }

                    await writer.WriteRequestAsync(connectRequest, cancellationToken: cancellationToken);

                    string httpStatus = await stream.ReadLineAsync(cancellationToken);

                    Response.ParseResponseLine(httpStatus, out _, out int statusCode, out string statusDescription);

                    if (statusCode != 200 && !statusDescription.EqualsIgnoreCase("OK")
                                          && !statusDescription.EqualsIgnoreCase("Connection Established"))
                    {
                        throw new Exception("Upstream proxy failed to create a secure tunnel");
                    }

                    await stream.ReadAndIgnoreAllLinesAsync(cancellationToken);
                }

                if (isHttps)
                {
                    var sslStream = new SslStream(stream, false, proxyServer.ValidateServerCertificate,
                        proxyServer.SelectClientCertificate);
                    stream = new CustomBufferedStream(sslStream, proxyServer.BufferPool, proxyServer.BufferSize);

                    var options = new SslClientAuthenticationOptions
                    {
                        ApplicationProtocols = applicationProtocols,
                        TargetHost = remoteHostName,
                        ClientCertificates = null,
                        EnabledSslProtocols = proxyServer.SupportedSslProtocols,
                        CertificateRevocationCheckMode = proxyServer.CheckCertificateRevocation
                    };
                    await sslStream.AuthenticateAsClientAsync(options, cancellationToken);
#if NETCOREAPP2_1
                    negotiatedApplicationProtocol = sslStream.NegotiatedApplicationProtocol;
#endif
                }
            }
            catch (Exception)
            {
                stream?.Dispose();
                tcpClient?.Close();
                throw;
            }

            return new TcpServerConnection(proxyServer, tcpClient)
            {
                UpStreamProxy = externalProxy,
                UpStreamEndPoint = upStreamEndPoint,
                HostName = remoteHostName,
                Port = remotePort,
                IsHttps = isHttps,
                NegotiatedApplicationProtocol = negotiatedApplicationProtocol,
                UseUpstreamProxy = useUpstreamProxy,
                StreamWriter = new HttpRequestWriter(stream, proxyServer.BufferPool, proxyServer.BufferSize),
                Stream = stream,
                Version = httpVersion
            };
        }


        /// <summary>
        ///     Release connection back to cache.
        /// </summary>
        /// <param name="connection">The Tcp server connection to return.</param>
        /// <param name="close">Should we just close the connection instead of reusing?</param>
        internal async Task Release(TcpServerConnection connection, bool close = false)
        {
            if (connection == null)
            {
                return;
            }

            if (close || connection.IsWinAuthenticated || !server.EnableConnectionPool)
            {
                disposalBag.Add(connection);
                return;
            }

            connection.LastAccess = DateTime.Now;

            try
            {
                await @lock.WaitAsync();

                while (true)
                {
                    if (cache.TryGetValue(connection.CacheKey, out var existingConnections))
                    {
                        while (existingConnections.Count >= server.MaxCachedConnections)
                        {
                            if (existingConnections.TryDequeue(out var staleConnection))
                            {
                                disposalBag.Add(staleConnection);
                            }
                        }

                        existingConnections.Enqueue(connection);
                        break;
                    }

                    if (cache.TryAdd(connection.CacheKey,
                        new ConcurrentQueue<TcpServerConnection>(new[] { connection })))
                    {
                        break;
                    }
                }

            }
            finally
            {
                @lock.Release();
            }
        }

        internal async Task Release(Task<TcpServerConnection> connectionCreateTask, bool closeServerConnection)
        {
            if (connectionCreateTask != null)
            {
                TcpServerConnection connection = null;
                try
                {
                    connection = await connectionCreateTask;
                }
                catch { }
                finally
                {
                    await Release(connection, closeServerConnection);
                }
            }
        }

        private async Task clearOutdatedConnections()
        {
            while (runCleanUpTask)
            {
                try
                {
                    foreach (var item in cache)
                    {
                        var queue = item.Value;

                        while (queue.Count > 0)
                        {
                            if (queue.TryDequeue(out var connection))
                            {
                                var cutOff = DateTime.Now.AddSeconds(-1 * server.ConnectionTimeOutSeconds);
                                if (!server.EnableConnectionPool
                                    || connection.LastAccess < cutOff)
                                {
                                    disposalBag.Add(connection);
                                    continue;
                                }

                                queue.Enqueue(connection);
                                break;
                            }
                        }
                    }

                    try
                    {
                        await @lock.WaitAsync();

                        //clear empty queues
                        var emptyKeys = cache.Where(x => x.Value.Count == 0).Select(x => x.Key).ToList();
                        foreach (string key in emptyKeys)
                        {
                            cache.TryRemove(key, out var _);
                        }
                    }
                    finally
                    {
                        @lock.Release();
                    }

                    while (!disposalBag.IsEmpty)
                    {
                        if (disposalBag.TryTake(out var connection))
                        {
                            connection?.Dispose();
                        }
                    }
                }
                finally
                {
                    //cleanup every 3 seconds by default
                    await Task.Delay(1000 * 3);
                }
              
            }
        }

        /// <summary>
        ///     Check if a TcpClient is good to be used.
        ///     This only checks if send is working so local socket is still connected.
        ///     Receive can only be verified by doing a valid read from server without exceptions.
        ///     So in our case we should retry with new connection from pool if first read after getting the connection fails.
        ///     https://msdn.microsoft.com/en-us/library/system.net.sockets.socket.connected(v=vs.110).aspx
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static bool isGoodConnection(TcpClient client)
        {
            var socket = client.Client;

            if (!client.Connected || !socket.Connected)
            {
                return false;
            }

            // This is how you can determine whether a socket is still connected.
            bool blockingState = socket.Blocking;
            try
            {
                var tmp = new byte[1];

                socket.Blocking = false;
                socket.Send(tmp, 0, 0);
                //Connected.
            }
            catch
            {
                //Should we let 10035 == WSAEWOULDBLOCK as valid connection?
                return false;
            }
            finally
            {
                socket.Blocking = blockingState;
            }

            return true;
        }

        public void Dispose()
        {
            runCleanUpTask = false;

            try
            {
                @lock.Wait();

                foreach (var queue in cache.Select(x => x.Value).ToList())
                {
                    while (!queue.IsEmpty)
                    {
                        if (queue.TryDequeue(out var connection))
                        {
                            disposalBag.Add(connection);
                        }
                    }
                }
                cache.Clear();
            }
            finally
            {
                @lock.Release();
            }

            while (!disposalBag.IsEmpty)
            {
                if (disposalBag.TryTake(out var connection))
                {
                    connection?.Dispose();
                }
            }
        }
    }
}

