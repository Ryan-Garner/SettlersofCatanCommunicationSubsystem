using log4net;
using Messages;
using UserApp.AppWorker;
using ConversationSubsystem;

namespace UserApp.UserAppConversations.JoinGameConversation
{
    public class JoinGameWaitForGameInfo : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JoinGameConversation));
        public JoinGameWaitForGameInfo(JoinGameConversation joinGameConversation)
            : base(joinGameConversation) { }

        public override void Execute()
        {
            Envelope e;
            if(myConversation.envelopeQueue.Count > 0)
            {
                if(myConversation.envelopeQueue.TryTake(out e))
                {
                    if(e.message.MessageType == Messages.Message.GAME_INFO)
                    {
                        ((UserAppWorker)myConversation.commFacility.myAppWorker).UserInfo.GameId = ((GameInfoMessage)e.message).GameId;
                        ((UserAppWorker)myConversation.commFacility.myAppWorker).UserInfo.GameManagerEP = e.remoteEndPoint;
                        myConversation.Stop();
                    }
                    else
                    {
                        Logger.Error("Join Game Conversation Expected a Game Info Message");
                    }
                }
            }
        }
    }
}
