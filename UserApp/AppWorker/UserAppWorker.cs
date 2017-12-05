using ConversationSubsystem;
using UserApp.AppWorker.States;

namespace UserApp.AppWorker
{
    public class UserAppWorker : ConversationSubsystem.AppWorker
    {
        public UserInfo UserInfo { get; protected set; }

        public AppState StartUpState { get; private set; }

        public UserAppWorker()
            : base(new UserAppConversationFactory())
        {
            UserInfo = new UserInfo();

            StartUpState = new StartUpState(this);
            currentState = StartUpState;
        }

        protected override void Init()
        {
            base.Init();

            commFacility.Initialize();
            commFacility.Start();
        }
    }
}
