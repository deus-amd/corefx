// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.WebSockets.Client.Tests
{
    /// <summary>
    /// ClientWebSocket tests that do require a remote server.
    /// </summary>
    public class ClientWebSocketTestBase
    {
        public readonly static object[][] EchoServers = Configuration.WebSockets.EchoServers;
        public readonly static object[][] EchoHeadersServers = Configuration.WebSockets.EchoHeadersServers;

        public const int TimeOutMilliseconds = 10000;
        public const int CloseDescriptionMaxLength = 123;
        public readonly ITestOutputHelper _output;

        public ClientWebSocketTestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> UnavailableWebSocketServers
        {
            get
            {
                Uri server;

                // Unknown server.
                {
                    server = new Uri(string.Format("ws://{0}", Guid.NewGuid().ToString()));
                    yield return new object[] { server };
                }

                // Known server but not a real websocket endpoint.
                {
                    server = Configuration.Http.RemoteEchoServer;
                    var ub = new UriBuilder("ws", server.Host, server.Port, server.PathAndQuery);

                    yield return new object[] { ub.Uri };
                }
            }
        }

        public async Task TestCancellation(Func<ClientWebSocket, Task> action, Uri server)
        {
            using (ClientWebSocket cws = await WebSocketHelper.GetConnectedWebSocket(server, TimeOutMilliseconds, _output))
            {
                try
                {
                    await action(cws);
                    // Operation finished before CTS expired.
                }
                catch (OperationCanceledException)
                {
                    // Expected exception
                    Assert.Equal(WebSocketState.Aborted, cws.State);
                }
                catch (ObjectDisposedException)
                {
                    // Expected exception
                    Assert.Equal(WebSocketState.Aborted, cws.State);
                }
                catch (WebSocketException exception)
                {
                    Assert.Equal(ResourceHelper.GetExceptionMessage(
                        "net_WebSockets_InvalidState_ClosedOrAborted",
                        "System.Net.WebSockets.InternalClientWebSocket",
                        "Aborted"),
                        exception.Message);

                    Assert.Equal(WebSocketError.InvalidState, exception.WebSocketErrorCode);
                    Assert.Equal(WebSocketState.Aborted, cws.State);
                }
            }
        }

        public static bool WebSocketsSupported { get { return WebSocketHelper.WebSocketsSupported; } }
    }
}
