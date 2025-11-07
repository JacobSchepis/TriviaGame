namespace TriviaGame.Phases
{
    public interface IGamePhase : IAsyncDisposable
    {
        // Called once when phase becomes active
        Task OnEnterAsync(LobbyContext ctx, CancellationToken ct);

        // Every client message is routed here while this phase is active
        Task HandleAsync(LobbyContext ctx, ClientMessage msg, CancellationToken ct);

        // Called once when the lobby advances away from this phase
        Task OnExitAsync(LobbyContext ctx, CancellationToken ct);

        object OnClientConnected();

        // Phase signals when it's ready to move on
        bool IsComplete { get; }
    }
}
