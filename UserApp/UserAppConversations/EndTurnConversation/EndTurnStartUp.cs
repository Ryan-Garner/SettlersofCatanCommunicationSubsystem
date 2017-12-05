using log4net;
using Messages;
using ConversationSubsystem;

namespace UserApp.UserAppConversations.EndTurnConversation
{
    public class EndTurnStartUp : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EndTurnConversation));
        private Envelope received;

        public EndTurnStartUp(EndTurnConversation endTurnConversation)
            : base(endTurnConversation) { }

        public override void Execute()
        {
            //myConversation.commFacility.Send(myConversation.FirstEnvelope);
            received = SendAndReceiveMessage(myConversation.FirstEnvelope);
            if(ReceivedValidMessage(received))
            {
                
                myConversation.SetState(((EndTurnConversation)myConversation).GetWaitForAck());
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
