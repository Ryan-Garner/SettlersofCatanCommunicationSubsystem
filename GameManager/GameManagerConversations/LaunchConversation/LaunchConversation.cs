using ConversationSubsystem;

namespace GameManager.GameManagerConversations.LaunchConversation
{
    public class LaunchConversation : Conversation
    {
        private ConversationState startUp;
        private ConversationState waitForGameInfo;

        public LaunchConversation(Envelope envelope, CommFacility commFacility)
            :base(envelope, commFacility)
        {
            startUp = new LaunchStartUp(this);
            waitForGameInfo = new LaunchWaitForGameInfo(this);
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
