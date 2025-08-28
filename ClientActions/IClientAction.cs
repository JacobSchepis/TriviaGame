using System.Collections;

namespace TriviaGame.ClientActions
{
    public interface IClientAction
    {
        string Action {  get; }
        void Execute(ClientMessage message, GameBehavior sender, TriviaLobby lobby);
    }

    public class TestAction : IClientAction
    {
        public string Action => "test";

        public void Execute(ClientMessage message, GameBehavior sender, TriviaLobby lobby)
        {
            
        }
    }
}
