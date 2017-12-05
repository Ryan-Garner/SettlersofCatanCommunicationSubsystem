using ConversationSubsystem;
using GameManager.AppWorker;
using log4net;
using Messages;
using System.Collections.Generic;
using System.Net;

namespace GameManager.GameManagerConversations.StartGameConversation
{
    public class StartGameUpdateState : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartGameUpdateState));

        /// <summary>
        /// Constructor, passes owning conversation to the base class
        /// </summary>
        /// <param name="conv">Conversation that owns this state</param>
        public StartGameUpdateState(StartGameConversation conv)
            : base(conv) { }

        /// <summary>
        /// Sends GameStateUpdateMessages to all the user apps and waits for them to respond with an ack.
        /// If everyone responds it will send a message to the starting player that everything is ready to go,
        /// otherwise it will end the conversation
        /// </summary>
        public override void Execute()
        {
            List<Envelope> envelopes = new List<Envelope>();
            GameInfo gameInfo = ((GameManagerAppWorker)myConversation.commFacility.myAppWorker).GameInfo;
            Message m = new GameStateUpdateMessage(myConversation.FirstEnvelope.message.convId, gameInfo.GameId, gameInfo.CurrentPlayerId, gameInfo.PlayerPoints, gameInfo.PlayerCardAmounts, gameInfo.BoardLayout, gameInfo.PlayerWithLR, gameInfo.LastMove);

            foreach (IPEndPoint ep in gameInfo.UserEndPoints.Values)
            {
                envelopes.Add(new Envelope(m, ep));
            }

            Envelope[] receivedEnvelopes = SendAndReceiveMultMessage(envelopes.ToArray());

            if (HaveReceivedFromAll(receivedEnvelopes))
            {
                IPEndPoint ep;
                gameInfo.UserEndPoints.TryGetValue(0, out ep);
                if(ep != null)
                {
                    SendMessage(new Envelope(new AckMessage(myConversation.FirstEnvelope.message.convId, 1), ep));
                }

                myConversation.Stop();
            }
            else
            {
                myConversation.Stop();
                // TODO: ERROR, didn't receive ACK FROM ALL
            }
        }

        /// <summary>
        /// Checks if the message received is one the state is expecting
        /// </summary>
        /// <param name="env">Message to check</param>
        /// <returns>true if it's an expected message</returns>
        public override bool ReceivedValidMessage(Envelope m)
        {
            return (m?.message?.MessageType == Message.ACK);
        }
    }
}
