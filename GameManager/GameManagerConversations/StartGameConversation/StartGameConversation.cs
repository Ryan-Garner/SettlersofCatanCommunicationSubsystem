using System.Threading.Tasks;
using ConversationSubsystem;

namespace GameManager.GameManagerConversations.StartGameConversation
{
    public class StartGameConversation : Conversation
    {
        public ConversationState StartUpState { get; private set; }
        public ConversationState GameStateUpdateState { get; private set; }

        /// <summary>
        /// Constructor, passes first envelope and managing CommFacility to base class
        /// </summary>
        /// <param name="envelope">firstEnvelope</param>
        /// <param name="commFacility">CommFacility that manages this conversation</param>
        public StartGameConversation(Envelope envelope, CommFacility commFacility)
            :base(envelope, commFacility)
        {
            StartUpState = new StartGameStartUpState(this);
            GameStateUpdateState = new StartGameUpdateState(this);
            currentState = StartUpState;
        }
    }
}
