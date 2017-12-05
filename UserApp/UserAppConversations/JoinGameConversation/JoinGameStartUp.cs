using ConversationSubsystem;

namespace UserApp.UserAppConversations.JoinGameConversation
{
    public class JoinGameStartUp : ConversationState
    {
        public JoinGameStartUp(JoinGameConversation joinGameConversation)
            : base(joinGameConversation) { }

        public override void Execute()
        {
            myConversation.commFacility.Send(myConversation.FirstEnvelope);
            myConversation.SetState(((JoinGameConversation)myConversation).GetWaitForGameInfo());
        }
    }
}
