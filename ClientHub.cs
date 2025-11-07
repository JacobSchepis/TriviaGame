namespace TriviaGame
{
    public class ClientHub
    {
        private readonly Dictionary<string, ClientBehavior> _clients = new();
        private readonly Dictionary<string, string> _clientLobby = new();

        public string RegisterClient(ClientBehavior behavior)
        {
            string newClientId = Guid.NewGuid().ToString();
            _clients[newClientId] = behavior;
            return newClientId;
        }

        public bool IsClientRegistered(string clientId)
        {
            return _clients.ContainsKey(clientId);
        }

        public void UnregisterClient(string clientId)
        {
            _clients.Remove(clientId);
        }

        //sending messages to clients
        public void SendMessageToClient(string clientId, string message)
        {
            if (_clients.TryGetValue(clientId, out var clientBehavior))
            {
                clientBehavior.SendMessage(message);
            }
        }



        public void AssociateClientWithLobby(string clientId, string lobbyId)
        {
            _clientLobby[clientId] = lobbyId;
        }

        public string? GetLobbyForClient(string clientId)
        {
            if (_clientLobby.TryGetValue(clientId, out var lobbyId))
            {
                return lobbyId;
            }
            return null;
        }
    }
}
