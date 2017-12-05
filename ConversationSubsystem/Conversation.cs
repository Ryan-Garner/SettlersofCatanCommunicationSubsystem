using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConversationSubsystem
{
    public abstract class Conversation
    {
        public Envelope FirstEnvelope { get; private set; }
        public BlockingCollection<Envelope> envelopeQueue;
        public CommFacility commFacility;

        public ConversationState currentState;

        private bool _keepGoing = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstEnvelope">First envelope of the conversation</param>
        /// <param name="commFacility">CommFacility that manages this conversation</param>
        public Conversation(Envelope firstEnvelope, CommFacility commFacility)
        {
            this.FirstEnvelope = firstEnvelope;
            this.commFacility = commFacility;
            envelopeQueue = new BlockingCollection<Envelope>();  
        }

        /// <summary>
        /// Start
        /// </summary>
        public virtual void Execute()
        {
            while (_keepGoing)
            {
                currentState.Execute();
            }
        }

        /// <summary>
        /// Runs the Execute() method on a seperate thread
        /// </summary>
        public virtual void Start()
        {
            _keepGoing = true;
            Task.Factory.StartNew(Execute);
        }

        /// <summary>
        /// Sets _keepGoing to false to stop the Execute method that's running on a seperate thread and
        /// removes the conversation from the managing CommFacility's dictionary
        /// </summary>
        public virtual void Stop()
        {
            commFacility.RemoveConversation(FirstEnvelope.message.convId);
            _keepGoing = false;
        }

        /// <summary>
        /// Adds an envelope to the conversation's queue
        /// </summary>
        /// <param name="e">Envelope to add to the queue</param>
        /// <returns>true if it succeeded, else false</returns>
        public bool Enqueue(Envelope e)
        {
            return envelopeQueue.TryAdd(e);
        }

        /// <summary>
        /// Changes currentState to the state passed in
        /// </summary>
        /// <param name="state">State to change currentState to</param>
        public void SetState(ConversationState state)
        {
            currentState = state;
        }
    }
}
