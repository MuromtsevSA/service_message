using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace service_message.HubMessage
{
    public class WebSocketServer
    {
        private readonly ConcurrentDictionary<WebSocket, bool> _clients;
        private readonly Channel<string> _broadcastChannel;
        private readonly ILogger<WebSocketServer> _logger;

        public WebSocketServer(ILogger<WebSocketServer> logger)
        {
            _clients = new ConcurrentDictionary<WebSocket, bool>();
            _broadcastChannel = Channel.CreateUnbounded<string>();
            _logger = logger;


            Task.Run(HandleMessages);
        }

        public void BroadcastMessage(string message)
        {

            _broadcastChannel.Writer.WriteAsync(message);
        }

        public async Task HandleConnections(HttpContext context)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _clients[socket] = true;
            _logger.LogInformation($"New client connected: {context.Connection.RemoteIpAddress}");

            await ReceiveMessages(socket);
        }

        private async Task ReceiveMessages(WebSocket socket)
        {
            var buffer = new byte[1024];

            while (_clients.TryGetValue(socket, out _))
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _clients.TryRemove(socket, out _);
                    _logger.LogInformation($"Client disconnected: {socket.CloseStatusDescription}");
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogInformation($"Received message from client: {message}");

    
                    BroadcastMessage(message);
                }
            }
        }

        private async Task HandleMessages()
        {
            while (await _broadcastChannel.Reader.WaitToReadAsync())
            {
                while (_broadcastChannel.Reader.TryRead(out var message))
                {
                    foreach (var client in _clients.Keys)
                    {
                        if (client.State == WebSocketState.Open)
                        {
                            var buffer = Encoding.UTF8.GetBytes(message);
                            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else
                        {
                            _clients.TryRemove(client, out _);
                        }
                    }
                }
            }
        }
    }
}
