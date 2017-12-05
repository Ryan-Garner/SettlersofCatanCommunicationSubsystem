using log4net;
using Messages;
using UserApp.AppWorker;
using ConversationSubsystem;

namespace UserApp.UserAppConversations.EndTurnConversation
{
    public class EndTurnWaitForAck : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EndTurnWaitForAck));
        private Envelope envelope;
        private Envelope received;

        public EndTurnWaitForAck(EndTurnConversation endTurnConversation)
            : base(endTurnConversation) { }

        public override void Execute()
        {
            UserInfo tempInfo = ((UserAppWorker)myConversation.commFacility.myAppWorker).UserInfo;
            envelope = new Envelope(new AckMessage(myConversation.FirstEnvelope.message.convId, tempInfo.GameId), tempInfo.GameManagerEP);
            received = SendAndReceiveMessage(envelope);
            if (ReceivedValidMessage(received))
            {
                myConversation.SetState(((EndTurnConversation)myConversation).GetWaitForAck());
            }
            else
            {
                Logger.Error("Didn't receive ACK");
            }
        }
        public override bool ReceivedValidMessage(Envelope env)
        {
            if (env.message.MessageType == Message.ACK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
