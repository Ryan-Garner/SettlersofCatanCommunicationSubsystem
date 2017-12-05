using ConversationSubsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registry.Conversations.StartGameConversation
{
    public class StartGameConversation : Conversation
    {
        public ConversationState AcknowledgeStartState { get; private set; }

        /// <summary>
        /// Constructor, passes first envelope and managing CommFacility to base class
        /// </summary>
        /// <param name="envelope">firstEnvelope</param>
        /// <param name="commFacility">CommFacility that manages this conversation</param>
        public StartGameConversation(Envelope envelope, CommFacility commFacility)
            : base(envelope, commFacility)
        {
            AcknowledgeStartState = new StartGameAcknowledgeStartState(this);
            currentState = AcknowledgeStartState;
        }
    }
}
