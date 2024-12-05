using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceA.Models;

namespace ServiceA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {
        // Implement Crud operations.
        private readonly ClientWebSocket _webSocket;

        public GraphController()
        {
            _webSocket = new ClientWebSocket();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGraph([FromBody] Graph graph)
        {
            await SendMessageToServiceBAsync("CreateGraph", graph);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetGraph(int id)
        {
            await SendMessageToServiceBAsync("GetGraph", id);
            return Ok();
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateGraph(int id, [FromBody] Graph graph)
        {
            graph.Id = id;
            await SendMessageToServiceBAsync("UpdateGraph", graph);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGraph(int id)
        {
            await SendMessageToServiceBAsync("DeleteGraph", id);
            return Ok();
        }


        private async Task SendMessageToServiceBAsync(string action, object data)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await _webSocket.ConnectAsync(new Uri("ws://localhost:5297/ws"), CancellationToken.None);
            }

            var message = new
            {
                Action = action,
                Data = data
            };

            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            var messageSegment = new ArraySegment<byte>(messageBytes);

            await _webSocket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

    }
}