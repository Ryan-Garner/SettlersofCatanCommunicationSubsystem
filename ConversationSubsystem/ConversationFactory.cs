namespace ConversationSubsystem
{
    public abstract class ConversationFactory
    {
        public CommFacility ManagingCommFacility { get; set; }

        public abstract Conversation Create(Envelope e);
        
    }
}
