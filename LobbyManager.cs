namespace TriviaGame
{
    public class LobbyManager
    {
        private Dictionary<string, TriviaLobby> _lobbyDictionary = new Dictionary<string, TriviaLobby>();

        public TriviaLobby? CreateLobby()
        {
            string id = Guid.NewGuid().ToString("N")[..6];
            var newLobby = new TriviaLobby(id);
            _lobbyDictionary[id] = newLobby;
            return newLobby;
        }

        public TriviaLobby? JoinLobby(string id)
        {
            if (!_lobbyDictionary.ContainsKey(id))
                return null;

            return _lobbyDictionary[id];
        }

        
    }

    
}
