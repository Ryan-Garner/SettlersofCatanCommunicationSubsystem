using ConversationSubsystem;
using log4net;
using Messages;
using Registry.AppWorker;
using SharedObjects;
using System.Net;

namespace Registry.Conversations.StartGameConversation
{
    public class StartGameAcknowledgeStartState : ConversationState
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
            if (ActivateGame(myConversation.FirstEnvelope))
            {
                IPEndPoint remoteEp = myConversation.FirstEnvelope.remoteEndPoint;
                MessageId convId = myConversation.FirstEnvelope.message.convId;
                short gameId = ((StartGameMessage)myConversation.FirstEnvelope.message).GameId;

                Envelope envelope = new Envelope(new AckMessage(convId, gameId), remoteEp);

                SendMessage(envelope);
            }
            else
            {
                // TODO: Error, received start game from a GameManager not registered with the Registry
            }

            myConversation.Stop();
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
        /// Activates the game that this end point maps to
        /// </summary>
        /// <param name="envelope">Envelope from GameManager that should be registered with Registry</param>
        /// <returns>true if it successfully activated game, else false</returns>
        private bool ActivateGame(Envelope envelope)
        {
            RegistryAppWorker userAppWorker = (RegistryAppWorker)myConversation.commFacility.myAppWorker;
            RegistryData.GameInfo game = userAppWorker.RegistryData.GetGame(envelope.remoteEndPoint);

            if (game != null)
            {
                game.GameActive = true;
                return true;
            }

            return false;
        }
    }
}
