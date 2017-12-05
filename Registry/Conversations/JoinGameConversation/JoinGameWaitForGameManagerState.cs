using ConversationSubsystem;
using Messages;
using log4net;
using Registry.AppWorker;

namespace Registry.Conversations.JoinGameConversation
{
    class JoinGameWaitForGameManagerState : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JoinGameWaitForGameManagerState));

        public JoinGameWaitForGameManagerState(JoinGameConversation conv)
            : base(conv) { }

        public override void Execute()
        {
            Envelope e;
            if (myConversation.envelopeQueue.Count > 0)
            {
                if (myConversation.envelopeQueue.TryTake(out e))
                {
                    if (e.message.MessageType == Message.ACK)
                    {
                        RegistryData.GameInfo gameInfo = new RegistryData.GameInfo();

                        gameInfo.GameActive = false;
                        gameInfo.GameId = RegistryData.GetNextGameId();
                        gameInfo.Players = 1;
                        gameInfo.RemoteEndPoint = e.remoteEndPoint;

                        if (((RegistryAppWorker)myConversation.commFacility.myAppWorker).RegistryData.AddGame(gameInfo))
                        {
                            ((JoinGameConversation)myConversation).SendGameInfoMessage(gameInfo);
                        }
                        else
                        {
                            Logger.Error("Registry was unable to add game to dictionary");
                        }

                        myConversation.Stop();
                    }
                    else
                    {
                        Logger.Error("Join Game Conversation Expected an Ack message but received message type " + e.message.MessageType);
                    }
                }
            }
        }
    }
}
