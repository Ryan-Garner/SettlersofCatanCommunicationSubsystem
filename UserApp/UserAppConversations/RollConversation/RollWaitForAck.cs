using log4net;
using Messages;
using UserApp.AppWorker;
using ConversationSubsystem;

namespace UserApp.UserAppConversations.RollConversation
{
    public class RollWaitForAck : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RollWaitForAck));
        private Envelope envelope;
        private Envelope received;

        public RollWaitForAck(RollConversation rollConversation)
            : base(rollConversation) { }

        public override void Execute()
        {
            UserInfo tempInfo = ((UserAppWorker)myConversation.commFacility.myAppWorker).UserInfo;
            envelope = new Envelope(new AckMessage(myConversation.FirstEnvelope.message.convId, tempInfo.GameId), tempInfo.GameManagerEP);
            received = SendAndReceiveMessage(envelope);
            if (ReceivedValidMessage(received))
            {
                myConversation.SetState(((RollConversation)myConversation).GetWaitForAck());
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
