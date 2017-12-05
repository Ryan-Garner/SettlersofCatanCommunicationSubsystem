namespace ConversationSubsystem
{
    public abstract class AppState
    {
        protected AppWorker myAppWorker;

        public AppState(AppWorker appWorker)
        {
            myAppWorker = appWorker;
        }

        public abstract void Execute();
    }
}
