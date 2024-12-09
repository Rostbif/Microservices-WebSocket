using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceA.Models;
using ServiceB.Models;
using ServiceB.Repositories;
using SharedLibrary.Models;

namespace ServiceB
{
    public class WebSocketHandler
    {
        private readonly GraphRepository _repository = new GraphRepository();

        public async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var message = JsonSerializer.Deserialize<WebSocketMessage>(messageJson);
                WebSocketMessage response = new WebSocketMessage();

                switch (message.Action)
                {
                    case "CreateGraph":
                        {
                            response = await CreateGraphAsync(message.Data);
                            break;
                        }
                    case "GetGraph":
                        {
                            response = await GetGraphAsync(webSocket, message.Data);
                            break;
                        }
                    case "UpdateGraph":
                        {
                            response = await UpdateGraphAsync(message.Data);
                            break;
                        }
                    case "DeleteGraph":
                        {
                            response = await DeleteGraphAsync(message.Data);
                            break;
                        }
                }

                var serializedResponse = JsonSerializer.Serialize(response);
                var responseBytes = Encoding.UTF8.GetBytes(serializedResponse);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                // Getting the next message
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                Console.WriteLine($"Reached Result: {result}");
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task<WebSocketMessage> CreateGraphAsync(object data)
        {
            var graph = JsonSerializer.Deserialize<Graph>(data.ToString());
            var graphs = await _repository.ReadGraphsAsync();
            graph.Id = graphs.Any() ? graphs.Max(g => g.Id) + 1 : 1;
            graphs.Add(graph);
            await _repository.WriteGraphsAsync(graphs);
            System.Console.WriteLine("Service B - Graph Created");
            return new WebSocketMessage { Status = "Success", Data = graph };
        }

        private async Task<WebSocketMessage> GetGraphAsync(WebSocket webSocket, object data)
        {
            var id = JsonSerializer.Deserialize<int>(data.ToString());
            var graphs = await _repository.ReadGraphsAsync();
            var graph = graphs.FirstOrDefault(g => g.Id == id);
            if (graph != null)
            {
                System.Console.WriteLine("Service B - Graph retrieved");
                return new WebSocketMessage { Status = "Success", Data = graph };
            }
            else
            {
                System.Console.WriteLine("Service B - Failed to retrieve the requested graph");
                return new WebSocketMessage { Status = "Failure", Data = "Failed to retrieve the requested graph" };
            }

        }

        private async Task<WebSocketMessage> UpdateGraphAsync(object data)
        {
            var graph = JsonSerializer.Deserialize<Graph>(data.ToString());
            var graphs = await _repository.ReadGraphsAsync();
            var index = graphs.FindIndex(g => g.Id == graph.Id);
            if (index != -1)
            {
                graphs[index] = graph;
                await _repository.WriteGraphsAsync(graphs);
                System.Console.WriteLine("Service B - Graph updated");
                return new WebSocketMessage { Status = "Success", Data = "Graph updated successfully" };
            }
            else
            {
                return new WebSocketMessage { Status = "Failure", Data = "Graph Not Found" };
            }
        }

        private async Task<WebSocketMessage> DeleteGraphAsync(object data)
        {
            var id = JsonSerializer.Deserialize<int>(data.ToString());
            var graphs = await _repository.ReadGraphsAsync();
            var graph = graphs.FirstOrDefault(g => g.Id == id);
            if (graph != null)
            {
                graphs.Remove(graph);
                await _repository.WriteGraphsAsync(graphs);
                System.Console.WriteLine("Service B - Graph Deleted");
                return new WebSocketMessage { Status = "Success", Data = "Graph removed successfully" };
            }
            else
            {
                return new WebSocketMessage { Status = "Failure", Data = "Graph Not Found" };

            }
        }

    }
}