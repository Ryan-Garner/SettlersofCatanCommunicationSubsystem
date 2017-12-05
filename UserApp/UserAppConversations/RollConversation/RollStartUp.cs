using log4net;
using Messages;
using ConversationSubsystem;

namespace UserApp.UserAppConversations.RollConversation
{
    public class RollStartUp : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RollConversation));
        private Envelope received;

        public RollStartUp(RollConversation rollConversation)
            : base(rollConversation) { }

        public override void Execute()
        {
            //myConversation.commFacility.Send(myConversation.FirstEnvelope);
            received = SendAndReceiveMessage(myConversation.FirstEnvelope);
            if(ReceivedValidMessage(received))
            {
                
                myConversation.SetState(((RollConversation)myConversation).GetWaitForAck());
            }
        }
        public override bool ReceivedValidMessage(Envelope env)
        {
            if(env.message.MessageType != Message.GAME_STATE_UPDATE)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
