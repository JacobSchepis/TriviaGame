using TriviaGame.ClientActions;

namespace TriviaGame
{
    public class GamePhase
    {
        private readonly PhaseType _phaseType;
        private Queue<GameServerAction> actions;
        private readonly TriviaLobby _lobby;
        private readonly AIService _aiService;

        public GamePhase(TriviaLobby lobby, AIService aiService, PhaseType phaseType)
        {
            _lobby = lobby;
            _aiService = aiService;
            _phaseType = phaseType;
        }

        /*
        create the structure. Ie all events triggers.
        For starting scene it will be a ai text for intro
        then introducing players/teams
        queue of text inputs are generated.

        */
        public Task OnEnterAsync(TriviaLobby lobby)
        {
            return Task.CompletedTask;
        }

        public Task OnExitAsync(TriviaLobby lobby) => Task.CompletedTask;

        /*
         moves to next item in queue
         */
        public Task ContinueSequence() => Task.CompletedTask;

        public void HandleMessage(ClientMessage msg, GameBehavior sender, TriviaLobby lobby)
        {

        }

        public bool IsComplete(TriviaLobby lobby) => false;

        // Actions allowed in this phase (optional for UI syncing)
        public IEnumerable<string> AllowedActions => Enumerable.Empty<string>();
    }

    public enum PhaseType
    {
        Intro,
        BasicTrivia,
        Outro
    }

    public abstract class GameServerAction
    {

    }

}
