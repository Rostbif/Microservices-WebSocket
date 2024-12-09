using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceA.Handlers;
using ServiceA.Models;
using SharedLibrary.Models;




namespace ServiceA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {
        // Implement Crud operations.
        private readonly WebSocketHandler _webSocketHandler;

        public GraphController(WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGraph([FromBody] Graph graph)
        {
            var responseJson = await _webSocketHandler.SendMessageToServiceBAsync("CreateGraph", graph);
            var responseObject = JsonSerializer.Deserialize<WebSocketMessage>(responseJson);

            if (responseObject.Status == "Success")
            {
                var createdGraph = JsonSerializer.Deserialize<Graph>(responseObject.Data.ToString());
                return CreatedAtAction(nameof(GetGraph), new { id = createdGraph.Id }, createdGraph); // HTTP 201 Created
            }
            else
            {
                return BadRequest(responseObject.Data); // HTTP 400 Bad Request
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGraph(int id)
        {
            var responseJson = await _webSocketHandler.SendMessageToServiceBAsync("GetGraph", id);
            var responseObject = JsonSerializer.Deserialize<WebSocketMessage>(responseJson);
            if (responseObject.Status == "Success")
            {
                //var graph = responseObject.Data as Graph;
                //var graph = JsonSerializer.Deserialize<Graph>(responseObject.Data.ToString());
                return Ok(responseObject.Data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateGraph(int id, [FromBody] Graph graph)
        {
            graph.Id = id;
            var responseJson = await _webSocketHandler.SendMessageToServiceBAsync("UpdateGraph", graph);
            var responseObject = JsonSerializer.Deserialize<WebSocketMessage>(responseJson);
            if (responseObject.Status == "Success")
            {
                return Ok();
            }
            else
            {
                return BadRequest(responseObject.Data); // HTTP 400 Bad Request
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGraph(int id)
        {
            var responseJson = await _webSocketHandler.SendMessageToServiceBAsync("DeleteGraph", id);
            var responseObject = JsonSerializer.Deserialize<WebSocketMessage>(responseJson);
            if (responseObject.Status == "Success")
            {
                return Ok();
            }
            else
            {
                return BadRequest(responseObject.Data); // HTTP 400 Bad Request
            }
        }


    }
}