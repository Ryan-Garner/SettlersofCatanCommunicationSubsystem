using log4net;
using ConversationSubsystem;

namespace GameManager.GameManagerConversations.LaunchConversation
{
    public class LaunchWaitForGameInfo : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LaunchConversation));

        public LaunchWaitForGameInfo(LaunchConversation launchConversation)
            : base(launchConversation) { }

        public override void Execute()
        {
            Envelope e;
            if (myConversation.envelopeQueue.Count > 0)
            {
                if(myConversation.envelopeQueue.TryTake(out e))
                {
                    if(e.message.MessageType == Messages.Message.GAME_INFO)
                    {
                        //TODO: Update Game Manager Info;
                        myConversation.Stop();
                        myConversation.commFacility.myAppWorker.Stop();
                    }
                    else
                    {
                        Logger.Info("Launch Conversation Expected a GameInfo Message");
                    }

                }
            }
        }
    }
}
