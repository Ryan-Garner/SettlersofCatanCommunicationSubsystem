using log4net;
using Messages;
using ConversationSubsystem;
using GameManager.AppWorker;

namespace GameManager.GameManagerConversations.EndTurnConversation
{
    public class EndTurnWaitForAcks : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EndTurnConversation));
        private Envelope Ack;

        public EndTurnWaitForAcks(EndTurnConversation endTurnConversation)
            : base(endTurnConversation)
        {
           
        }

        public override void Execute()
        {
            AckMessage Continue = new AckMessage(myConversation.FirstEnvelope.message.convId, ((GameManagerAppWorker)myConversation.commFacility.myAppWorker).GameInfo.GameId);
            Ack = new Envelope(Continue, myConversation.FirstEnvelope.remoteEndPoint);
            SendMessage(Ack);
            myConversation.Stop();
        }
        public override bool ReceivedValidMessage(Envelope env)
        {
            if (env.message.MessageType != Message.ACK)
            {
                return false;
            }
            return true;
        }
    }
}
