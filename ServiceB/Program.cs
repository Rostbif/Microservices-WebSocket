using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebSockets;
using ServiceB.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
// TBD - check if I need that
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(120);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseWebSockets();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    Console.WriteLine("Service B got a request");
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var handler = new WebSocketHandler();
            await handler.HandleWebSocketAsync(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.Run();

public class WebSocketHandler
{
    public async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        while (!result.CloseStatus.HasValue)
        {
            // var serverMsg = Encoding.UTF8.GetBytes($"Server: {DateTime.Now}");
            // await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
            // result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var message = JsonSerializer.Deserialize<WebSocketMessage>(messageJson);

            switch (message.Action)
            {
                case "CreateGraph":
                    {
                        Console.WriteLine("Service B - Create Graph reached");
                        break;
                    }
                case "GetGraph":
                    {
                        Console.WriteLine("Service B - Get Graph reached");
                        break;
                    }
                case "UpdateGraph":
                    {
                        Console.WriteLine("Service B - Update Graph reached");
                        break;
                    }
                case "DeleteGraph":
                    {
                        Console.WriteLine("Service B - Delete Graph reached");
                        break;
                    }
            }

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            Console.WriteLine($"Reached Result: {result}");


        }
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}

