using TriviaGame;
using WebSocketSharp.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<LobbyManager>();

var app = builder.Build();

var lobbyManager = app.Services.GetRequiredService<LobbyManager>();

var wssv = new WebSocketServer("ws://0.0.0.0:5000");
wssv.AddWebSocketService("/ws", () =>
{
    return new GameBehavior
    {
        LobbyManager = lobbyManager
    };
});
wssv.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
