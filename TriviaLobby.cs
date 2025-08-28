using TriviaGame.ClientActions;

namespace TriviaGame
{
    public class TriviaLobby
    {
        private Dictionary<string, GameBehavior> clients = new Dictionary<string, GameBehavior>();
        private Dictionary<string, TriviaTeam> teams = new Dictionary<string, TriviaTeam>();

        public int PlayerCount { get => clients.Count - 1; }

        private Queue<GamePhase> _phases;
        private GamePhase _currentPhase;

        private AIService _aiService;

        



        public string Id { get; private set; }

        public TriviaLobby(string id)
        {
            Id = id;
            Console.WriteLine("new lobby is created");
        }

        public void JoinAsHost(GameBehavior gameBehavior)
        {
            if (clients.ContainsKey("host")) return;

            clients["host"] = gameBehavior;
        }

        public bool JoinLobby(string playerName, GameBehavior gameBehavior)
        {
            if (playerName is null || gameBehavior is null)
                return false;

            if (clients.ContainsKey(playerName))
                return false;

            clients.Add(playerName, gameBehavior);
            Console.WriteLine($"Player joined: {playerName}");
            return true;
        }

        public void RecieveMessage(ClientMessage msg, GameBehavior sender)
        {
            switch (msg.Action)
            {
                case "leave":
                    break;

                default:
                    HandleClientMessage(msg, sender);
                    break;
            }
        }

        private void SendMessageToHost(object msg)
        {

        }

        private void SendMessageToAllPlayers(object msg)
        {

        }

        private void SendMessageToPlayer(object msg, string playerName)
        {

        }

        #region game stuff
        public async Task StartNextPhaseAsync()
        {
            _currentPhase = _phases.Dequeue();

            var aiLine = await _currentPhase.GetAIDialogueAsync(this, _aiService);
            if (aiLine != null)
                await _aiService.PlayVoiceLineAsync(aiLine);

            await _currentPhase.OnEnterAsync(this);
        }

        public void HandleClientMessage(ClientMessage msg, GameBehavior sender)
        {
            _currentPhase?.HandleMessage(msg, sender, this);

            if (_currentPhase?.IsComplete(this) == true)
                _ = StartNextPhaseAsync();
        }

        public void BroadcastClientEvent(string v, object value)
        {
            throw new NotImplementedException();
        }





        #endregion
    }



    public class TriviaTeam
    {
        public List<string> Members = new List<string>();
        public string TeamName = "";
        public int Points = 0;

        public void AddMember(string member)
        {
            if (Members.Contains(member)) return;
            Members.Add(member);
        }

        public void RemoveMember(string member)
        {
            if (!Members.Contains(member)) return;
            Members.Remove(member);
        }
    }

    public class Player
    {
        public string Name;
    }

    public enum LobbyState
    {
        Open,
        InGame,
        Finished
    }

}
