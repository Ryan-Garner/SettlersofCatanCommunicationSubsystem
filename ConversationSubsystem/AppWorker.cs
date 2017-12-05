namespace ConversationSubsystem
{
    public abstract class AppWorker
    {
        public AppState currentState;
        public CommFacility commFacility;

        private bool _keepGoing;

        public AppWorker(ConversationFactory factory)
        {
            commFacility = new CommFacility(this, factory);
            _keepGoing = false;
        }

        protected virtual void Init()
        { 
            _keepGoing = true;
        }

        public void Start()
        {
            Init();

            while (_keepGoing)
            {
                currentState?.Execute();
            }

            commFacility.Stop();
        }

        public void Stop()
        {
            _keepGoing = false;
        }

        public void SetState(AppState state)
        {
            currentState = state;
        }
    }
}
