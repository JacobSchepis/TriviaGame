using System.Runtime.InteropServices;
using TriviaGame.Phases;

namespace TriviaGame
{
    public class TriviaLobby
    {
        public string Id { get; private set; }

        private readonly SemaphoreSlim _gate = new(1, 1);
        private readonly Queue<IGamePhase> _phases = new();
        private IGamePhase? _currentPhase;
        private readonly LobbyContext _ctx;

        public TriviaLobby(string id)
        {
            Id = id;
            _ctx = new LobbyContext(id);
            Console.WriteLine("new lobby is created");
        }

        public void JoinAsHost(string clientId)
        {
            _ctx.HostId = clientId;
        }

        public bool JoinLobby(string playerId)
        {
            if (playerId is null)
                return false;

            if (_ctx.Clients.Contains(playerId))
                return false;

            _ctx.Clients.Add(playerId);
            Console.WriteLine($"Player joined: {playerId}");
            return true;
        }

        public async Task StartAsync(CancellationToken ct = default)
        {
            await _gate.WaitAsync(ct);
            try
            {
                if (_currentPhase != null) return;
                await AdvancePhase_NoLock(ct);
            }
            finally { _gate.Release(); }
        }

        public async Task RecieveClientMessage(ClientMessage msg, CancellationToken ct = default)
        {

            // (2) Route to the current phase
            await _gate.WaitAsync(ct);
            try
            {
                if (_currentPhase != null)
                {
                    await _currentPhase.HandleAsync(_ctx, msg, ct);
                    if (_currentPhase.IsComplete)
                        await AdvancePhase_NoLock(ct);
                }
                // else: ignore or buffer, depending on your needs
            }
            finally { _gate.Release(); }
        }

        private async Task AdvancePhase_NoLock(CancellationToken ct)
        {
            // exit previous
            if (_currentPhase != null)
            {
                await _currentPhase.OnExitAsync(_ctx, ct);
                await _currentPhase.DisposeAsync();
                _currentPhase = null;
            }

            if (_phases.Count == 0)
                return; // no more phases

            _currentPhase = _phases.Dequeue();
            await _currentPhase.OnEnterAsync(_ctx, ct);
        }

    }

    public sealed class LobbyContext
    {
        public string LobbyId { get; }
        public string HostId { get; set; } = "";
        public List<string> Clients { get; } = new();
        public Dictionary<string, string> Players { get; } = new();
        public int QuestionIndex { get; set; }
        public Dictionary<string, int> Scores { get; } = new();
        public Dictionary<string, string> LatestAnswers { get; } = new();

        public Action<string, object>? SendTo;
        public Action<object>? SendAll;

        public LobbyContext(string lobbyId) => LobbyId = lobbyId;
    }
}
