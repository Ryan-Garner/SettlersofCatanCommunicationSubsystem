using ConversationSubsystem;

namespace GameManager.GameManagerConversations.LaunchConversation
{
    public class LaunchStartUp : ConversationState
    {
        public LaunchStartUp(LaunchConversation launchConversation)
            : base(launchConversation) { }

        public override void Execute()
        {
            myConversation.commFacility.Send(myConversation.FirstEnvelope);
            myConversation.SetState(((LaunchConversation)myConversation).GetWaitForGameInfo());
        }
    }
}
