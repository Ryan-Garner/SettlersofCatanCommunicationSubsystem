using ConversationSubsystem;

namespace UserApp.UserAppConversations.JoinGameConversation
{
    public class JoinGameConversation : Conversation
    {
        private ConversationState startUp;
        private ConversationState waitForGameInfo;

        public JoinGameConversation(Envelope envelope, CommFacility commSubsystem) 
            : base(envelope, commSubsystem)
        {
            startUp = new JoinGameStartUp(this);
            waitForGameInfo = new JoinGameWaitForGameInfo(this);
            currentState = startUp;
        }
        
        public ConversationState GetStartUp()
        {
            return startUp;
        }
        public ConversationState GetWaitForGameInfo()
        {
            return waitForGameInfo;
        }
    }
}
