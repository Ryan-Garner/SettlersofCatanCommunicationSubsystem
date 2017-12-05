using System.Net;
using log4net;
using Messages;
using ConversationSubsystem;
using GameManager.AppWorker;

namespace GameManager.GameManagerConversations.RollConversation
{
    public class RollStartUp : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RollConversation));
        private Envelope[] envelopes = new Envelope[4];
        private Envelope[] receivedEnvelopes = new Envelope[4];
        private IPEndPoint ep;

        public RollStartUp(RollConversation rollConversation)
            : base(rollConversation) { }

        public override void Execute()
        {
            //TODO: Update game data
            GameStateUpdateMessage updateMessage;
            GameManagerAppWorker tempWorker = ((GameManagerAppWorker)myConversation.commFacility.myAppWorker);
            
            foreach (IPEndPoint userEndPoint in tempWorker.GameInfo.UserEndPoints.Values)
            {
                for (short i = 0; i < 4; i++)
                {
                    updateMessage = new GameStateUpdateMessage(myConversation.FirstEnvelope.message.convId, tempWorker.GameInfo.GameId, i, tempWorker.GameInfo.PlayerPoints, tempWorker.GameInfo.PlayerCardAmounts, tempWorker.GameInfo.BoardLayout, tempWorker.GameInfo.PlayerWithLR, tempWorker.GameInfo.LastMove);
                    tempWorker.GameInfo.UserEndPoints.TryGetValue((int)i, out ep);
                    envelopes[i] = new Envelope(updateMessage, ep);
                }
            }
            receivedEnvelopes = SendAndReceiveMultMessage(envelopes);
            if(HaveReceivedFromAll(receivedEnvelopes) && receivedAcks(receivedEnvelopes))
            {
                myConversation.SetState(((RollConversation)myConversation).GetWaitForAcks());
            }
            else
            {
                Logger.Error("Failed to receive the correct ack from all users");
            }

            
        }

        private bool receivedAcks(Envelope[] received)
        {
            foreach(Envelope envelope in received)
            {
                if (envelope.message.MessageType != Message.ACK)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
