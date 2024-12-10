using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SharedLibrary.Models;

namespace ServiceA.Handlers
{
    public class WebSocketHandler
    {
        private readonly ClientWebSocket _webSocket;
        private readonly string _webSocketUri;


        public WebSocketHandler()
        {
            _webSocket = new ClientWebSocket();
            // url to use in a docker environment: ws://serviceb:5297/ws
            // url to use when running it locally: ws://localhost:5297/ws
            _webSocketUri = Environment.GetEnvironmentVariable("WEBSOCKET_URI") ?? "ws://localhost:5297/ws";
            Console.WriteLine($"Service A: The _webSockerUri: {_webSocketUri}");
        }


        public async Task<string> SendMessageToServiceBAsync(string action, object data)
        {
            if (_webSocket.State != WebSocketState.Open)
            {

                await _webSocket.ConnectAsync(new Uri(_webSocketUri), CancellationToken.None);
            }

            var message = new WebSocketMessage
            {
                Action = action,
                Status = string.Empty,
                Data = data,
            };

            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            var messageSegment = new ArraySegment<byte>(messageBytes);

            await _webSocket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            // Recieve the response
            var buffer = new byte[1024 * 4];
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            return responseJson;
        }
    }
}