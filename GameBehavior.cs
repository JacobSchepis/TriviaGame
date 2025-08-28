using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TriviaGame
{
    public class GameBehavior : WebSocketBehavior
    {
        public required LobbyManager LobbyManager;
        private TriviaLobby? myLobby;
        public string MyPlayerId;

        protected override void OnOpen()
        {
            base.OnOpen();

            Console.WriteLine("New websocket connection");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            base.OnError(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);

            Send("Message recieved");

            string json = e.Data;
            var message = JsonSerializer.Deserialize<ClientMessage>(json);

            if (message is null)
            {
                Send("Invalid Json message");
                return;
            }

            switch (message.Action)
            {
                case "join":
                    JoinLobby(message);
                    break;

                case "host":
                    CreateLobby(message);
                    break;
                default:
                    SendMessageToLobby(message);
                    break;
            }
        }

        private void SendMessageToLobby(ClientMessage message)
        {
            if (myLobby is null) return;
            myLobby.RecieveMessage(message);
        }

        public void JoinLobby(ClientMessage message)
        {
            var lobby = LobbyManager.JoinLobby(message.LobbyCode);

            if (lobby is null || message.PlayerName is null) return;

            var completed = lobby.JoinLobby(message.PlayerName, this);

            if (completed is false)
            {
                Send("couldnt join lobby");
                return;
            }
            
            myLobby = lobby;
            myPlayerId = message.PlayerName;
        }

        private void CreateLobby(ClientMessage message)
        {
            var lobby = LobbyManager.CreateLobby();
            
            if (lobby is null) return;
            myLobby = lobby;
            myLobby.JoinAsHost(this);
            Send(myLobby.Id);
        }

        public void SendMessage(string msg)
        {
            Send(msg);
        }
    }

    public class ClientMessage
    {
        public string Action { get; set; }
        public string PlayerName { get; set; }
        public string LobbyCode { get; set; }

        public string Message { get; set; }

        public string Answer {  get; set; }
    }
}
