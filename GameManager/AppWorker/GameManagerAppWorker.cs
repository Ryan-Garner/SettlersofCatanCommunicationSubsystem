using ConversationSubsystem;
using GameManager.AppWorker.States;

namespace GameManager.AppWorker
{
    public class GameManagerAppWorker : ConversationSubsystem.AppWorker
    {
        public GameInfo GameInfo { get; protected set; }

        public AppState StartUpState { get; private set; }
        public TempState TempState { get; private set; }

        public GameManagerAppWorker(string[] args)
            : base(new GameManagerConversationFactory())
        {
            GameInfo = new GameInfo();

            StartUpState = new StartUpState(this, args);
            TempState = new TempState(this);
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
