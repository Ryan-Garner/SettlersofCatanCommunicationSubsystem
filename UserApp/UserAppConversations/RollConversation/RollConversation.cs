using ConversationSubsystem;

namespace UserApp.UserAppConversations.RollConversation
{
    public class RollConversation : Conversation
    {
        private ConversationState startUp;
        private ConversationState waitForAck;

        public RollConversation(Envelope envelope, CommFacility commSubsystem)
            : base(envelope, commSubsystem)
        {
            startUp = new RollStartUp(this);
            waitForAck = new RollWaitForAck(this);
            currentState = startUp;
            

        }

        public ConversationState GetWaitForAck()
        {
            return waitForAck;
        }
    }
}
