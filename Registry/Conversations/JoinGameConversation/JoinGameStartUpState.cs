using ConversationSubsystem;
using System.Diagnostics;
using System.IO;
using log4net;
using Registry.AppWorker;

namespace Registry.Conversations.JoinGameConversation
{
    public class JoinGameStartUpState : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JoinGameStartUpState));

        public JoinGameStartUpState(JoinGameConversation conv)
            : base(conv) { }

        public override void Execute()
        {
            RegistryData.GameInfo gameInfo = ((RegistryAppWorker)myConversation.commFacility.myAppWorker).RegistryData.GetAvailableGameManager();
            
            if (gameInfo == null)
            {
                LaunchCommandLineApp();
                myConversation.currentState = ((JoinGameConversation)myConversation).WaitForGameManagerState;
            }
            else
            {
                ((JoinGameConversation)myConversation).SendGameInfoMessage(gameInfo);
            }
        }

        private void LaunchCommandLineApp()
        {
            string solutionDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string exeDir = solutionDir + @"\GameManager\bin\Debug\";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                //UseShellExecute = false,
                FileName = exeDir + "GameManager.exe",
                //WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = myConversation.FirstEnvelope.message.convId.Pid + " " + myConversation.FirstEnvelope.message.convId.Seq
            };

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                Logger.Error("Can't start GameManager.exe");
            }
        }
    }
}
