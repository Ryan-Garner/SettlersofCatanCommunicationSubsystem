using ConversationSubsystem;

namespace UserApp.UserAppConversations.StartGameConversation
{
    public class StartGameConversation : Conversation
    {
        public ConversationState AcknowledgeStartState { get; private set; }
        public ConversationState AcknowledgeGameUpdateState { get; private set; }
        public ConversationState WaitForPlayerStartAckState { get; private set; }

        /// <summary>
        /// Constructor, passes first envelope and managing CommFacility to base class
        /// </summary>
        /// <param name="envelope">firstEnvelope</param>
        /// <param name="commFacility">CommFacility that manages this conversation</param>
        public StartGameConversation(Envelope envelope, CommFacility commFacility)
            : base(envelope, commFacility)
        {
            AcknowledgeStartState = new StartGameAcknowledgeStartState(this);
            AcknowledgeGameUpdateState = new StartGameAcknowledgeGameUpdateState(this);
            WaitForPlayerStartAckState = new StartGameAcknowledgeGameUpdateState(this);
            currentState = AcknowledgeStartState;
        }
    }
}
