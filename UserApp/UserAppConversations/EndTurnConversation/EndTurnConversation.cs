using ConversationSubsystem;

namespace UserApp.UserAppConversations.EndTurnConversation
{
    public class EndTurnConversation : Conversation
    {
        private ConversationState startUp;
        private ConversationState waitForAck;

        public EndTurnConversation(Envelope envelope, CommFacility commSubsystem)
            : base(envelope, commSubsystem)
        {
            startUp = new EndTurnStartUp(this);
            waitForAck = new EndTurnWaitForAck(this);
            currentState = startUp;
            

        }

        public ConversationState GetWaitForAck()
        {
            return waitForAck;
        }
    }
}
