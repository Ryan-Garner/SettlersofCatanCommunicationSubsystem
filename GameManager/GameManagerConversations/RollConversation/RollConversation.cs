using ConversationSubsystem;

namespace GameManager.GameManagerConversations.RollConversation
{
    public class RollConversation : Conversation
    {
        private ConversationState startUp;
        private ConversationState waitForAcks;

        public RollConversation(Envelope envelope, CommFacility commSubsystem) 
            : base(envelope, commSubsystem)
        {
            startUp = new RollStartUp(this);
            waitForAcks = new RollWaitForAcks(this);
            currentState = startUp;
        }

        public ConversationState GetWaitForAcks()
        {
            return waitForAcks;
        }
    }
}
