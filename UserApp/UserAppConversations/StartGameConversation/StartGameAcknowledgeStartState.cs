using ConversationSubsystem;
using log4net;
using Messages;
using SharedObjects;
using System.Collections.Generic;
using System.Net;
using UserApp.AppWorker;

namespace UserApp.UserAppConversations.StartGameConversation
{
    class StartGameAcknowledgeStartState : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartGameAcknowledgeStartState));

        /// <summary>
        /// Constructor, passes owning conversation to the base class
        /// </summary>
        /// <param name="conv">Conversation that owns this state</param>
        public StartGameAcknowledgeStartState(StartGameConversation conv)
            : base(conv) { }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            IPEndPoint remoteEp = myConversation.FirstEnvelope.remoteEndPoint;
            MessageId convId = myConversation.FirstEnvelope.message.convId;
            short gameId = ((StartGameMessage)myConversation.FirstEnvelope.message).GameId;

            Envelope envelope = new Envelope(new AckMessage(convId, gameId), remoteEp);

            Envelope envelopeReceived = SendAndReceiveMessage(envelope);

            if (envelopeReceived?.message != null)
            {
                GameStateUpdateMessage receivedGameState = (GameStateUpdateMessage)envelopeReceived.message;
                ((StartGameAcknowledgeGameUpdateState)((StartGameConversation)myConversation).AcknowledgeGameUpdateState).ReceivedGameState = receivedGameState;

                SetUserInfo(receivedGameState);

                myConversation.SetState(((StartGameConversation)myConversation).AcknowledgeGameUpdateState);
            }
            else
            {
                myConversation.Stop();
                // TODO: ERROR, didn't receive GameStateUpdateMessage
            }
        }

        /// <summary>
        /// Checks if the message received is one the state is expecting
        /// </summary>
        /// <param name="env">Message to check</param>
        /// <returns>true if it's an expected message</returns>
        public override bool ReceivedValidMessage(Envelope env)
        {
            return (env?.message?.MessageType == Message.GAME_STATE_UPDATE);
        }

        /// <summary>
        /// Assigns received game info data to the UserAppWorker's user info
        /// </summary>
        /// <param name="gMessage">GameStateUpdateMessage received</param>
        private void SetUserInfo(GameStateUpdateMessage gMessage)
        {
            UserAppWorker userAppWorker = (UserAppWorker)myConversation.commFacility.myAppWorker;
            userAppWorker.UserInfo.GameId = gMessage.GameId;
            userAppWorker.UserInfo.CurrentPlayerId = gMessage.CurrentPlayerId;
            userAppWorker.UserInfo.PlayerPoints = gMessage.PlayerPoints;
            userAppWorker.UserInfo.PlayerCardAmounts = gMessage.PlayerCardsAmounts;
            userAppWorker.UserInfo.BoardLayout = gMessage.BoardLayout;
            userAppWorker.UserInfo.PlayerWithLR = gMessage.PlayerWithLR;
            userAppWorker.UserInfo.LastMove = gMessage.LastMove;
        }
    }
}
