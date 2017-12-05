using ConversationSubsystem;
using log4net;
using Messages;
using SharedObjects;
using System.Collections.Generic;
using System.Net;
using UserApp.AppWorker;

namespace UserApp.UserAppConversations.StartGameConversation
{
    class StartGameAcknowledgeGameUpdateState : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartGameAcknowledgeGameUpdateState));

        public GameStateUpdateMessage ReceivedGameState { private get; set; }

        /// <summary>
        /// Constructor, passes owning conversation to the base class
        /// </summary>
        /// <param name="conv">Conversation that owns this state</param>
        public StartGameAcknowledgeGameUpdateState(StartGameConversation conv)
            : base(conv)
        {
            ReceivedGameState = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            IPEndPoint remoteEp = myConversation.FirstEnvelope.remoteEndPoint;
            MessageId convId = myConversation.FirstEnvelope.message.convId;
            short gameId = ((StartGameMessage)myConversation.FirstEnvelope.message).GameId;

            Envelope envelope = new Envelope(new AckMessage(convId, gameId), remoteEp);
            
            UserInfo userInfo = ((UserAppWorker)myConversation.commFacility.myAppWorker).UserInfo;
            UserAppWorker appWorker = (UserAppWorker)myConversation.commFacility.myAppWorker;

            if (ReceivedGameState?.CurrentPlayerId == userInfo.PlayerId)
            {
                Envelope receivedEnvelope = SendAndReceiveMessage(envelope);

                if (receivedEnvelope?.message != null)
                {
                    appWorker.UserInfo.AcceptUserInput = true;
                }
                else
                {
                    // TODO: Throw Error, no Start Ack Message received
                }
            }
            else
            {
                // TODO: Handle what happens if this ack is not received and so another GameUpdateStateMessage was received
                SendMessage(envelope);
            }

            myConversation.Stop();
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
