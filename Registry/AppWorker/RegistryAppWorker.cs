using ConversationSubsystem;
using Registry.AppWorker.States;
using Registry.Conversations;
using System.Net;

namespace Registry.AppWorker
{
    public class RegistryAppWorker : ConversationSubsystem.AppWorker
    {
        public RegistryData RegistryData { get; protected set; }

        public AppState StartUpState { get; protected set; }

        public RegistryAppWorker() 
            : base(new RegistryConversationFactory())
        {
            RegistryData = new RegistryData();

            StartUpState = new StartUpState(this);
            currentState = StartUpState;
        }

        protected override void Init()
        {
            base.Init();

            commFacility.Initialize();

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            commFacility.Start(localEndPoint);

            System.Console.WriteLine("Listening on " + localEndPoint.ToString());
        }
    }
}
