using ConversationSubsystem;

namespace GameManager.GameManagerConversations.EndTurnConversation
{
    public class EndTurnConversation : Conversation
    {
        private ConversationState startUp;
        private ConversationState waitForAcks;

        public EndTurnConversation(Envelope envelope, CommFacility commSubsystem) 
            : base(envelope, commSubsystem)
        {
            startUp = new EndTurnStartUp(this);
            waitForAcks = new EndTurnWaitForAcks(this);
            currentState = startUp;
        }

        public ConversationState GetWaitForAcks()
        {
            return waitForAcks;
        }
    }
}
