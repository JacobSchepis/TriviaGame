using TriviaGame;
using TriviaGame.Phases;

public sealed class LobbyWaitingPhase : IGamePhase
{
    public string Name => "LobbyWaiting";
    public bool IsComplete { get; private set; }

    private readonly int _minPlayers;
    private readonly TimeSpan _timeout;
    private CancellationTokenSource? _cts;
    private Task? _timerTask;

    public LobbyWaitingPhase(int minPlayers, TimeSpan timeout)
    {
        _minPlayers = minPlayers;
        _timeout = timeout;
    }

    public Task OnEnterAsync(LobbyContext ctx, CancellationToken ct)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        // timer that completes the phase after timeout if enough players have joined
        _timerTask = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_timeout, _cts.Token);
                if (ctx.Players.Count >= _minPlayers)
                    IsComplete = true;
            }
            catch { /* cancelled */ }
        }, _cts.Token);

        return Task.CompletedTask;
    }

    public Task HandleAsync(LobbyContext ctx, ClientMessage msg, CancellationToken ct)
    {
        // Example schema: msg.Action == "hostStart" OR "playerJoined"
        if (msg.Action == "playerJoined")
        {
            // you likely add to ctx.Players elsewhere; shown for completeness
            if (!string.IsNullOrEmpty(msg.ClientId) && 
                ctx.Clients.Contains(msg.ClientId) &&
               !ctx.Players.ContainsKey(msg.ClientId) == false)
            {
                ctx.Players[msg.ClientId] = msg.PlayerName;
            }
                    
        }
        else if (msg.Action == "hostStart")
        {
            IsComplete = true;
        }

        return Task.CompletedTask;
    }

    public async Task OnExitAsync(LobbyContext ctx, CancellationToken ct)
    {
        _cts?.Cancel();
        if (_timerTask != null) { try { await _timerTask; } catch { } }
        ctx.SendAll?.Invoke(new { type = "lobby", status = "starting" });
    }

    public ValueTask DisposeAsync()
    {
        _cts?.Dispose();
        return ValueTask.CompletedTask;
    }

    public object OnClientConnected()
    {
        return new();
    }
}
