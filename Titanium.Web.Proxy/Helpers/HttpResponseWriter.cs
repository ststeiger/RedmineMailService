﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StreamExtended;
using Titanium.Web.Proxy.Http;

namespace Titanium.Web.Proxy.Helpers
{
    internal sealed class HttpResponseWriter : HttpWriter
    {
        internal HttpResponseWriter(Stream stream, IBufferPool bufferPool, int bufferSize) 
            : base(stream, bufferPool, bufferSize)
        {
        }

        /// <summary>
        ///     Writes the response.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="flush">Should we flush after write?</param>
        /// <param name="cancellationToken">Optional cancellation token for this async task.</param>
        /// <returns>The Task.</returns>
        internal async Task WriteResponseAsync(Response response, bool flush = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await WriteResponseStatusAsync(response.HttpVersion, response.StatusCode, response.StatusDescription,
                cancellationToken);
            await WriteAsync(response, flush, cancellationToken);
        }

        /// <summary>
        ///     Write response status
        /// </summary>
        /// <param name="version">The Http version.</param>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <param name="cancellationToken">Optional cancellation token for this async task.</param>
        /// <returns>The Task.</returns>
        internal Task WriteResponseStatusAsync(Version version, int code, string description,
            CancellationToken cancellationToken)
        {
            return WriteLineAsync(Response.CreateResponseLine(version, code, description), cancellationToken);
        }
    }
}
