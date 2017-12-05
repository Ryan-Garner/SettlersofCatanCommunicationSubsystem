using ConversationSubsystem;
using Messages;

namespace Registry.Conversations
{
    public class RegistryConversationFactory : ConversationFactory
    {
        public override Conversation Create(Envelope e)
        {
            Conversation c = null;
            switch (e.message.MessageType)
            {
                case Message.REQUEST_GAME:
                    c = new JoinGameConversation.JoinGameConversation(e, ManagingCommFacility);
                    break;
                case Message.START_GAME:
                    c = new StartGameConversation.StartGameConversation(e, ManagingCommFacility);
                    break;
                    // TODO: Add cases for the other conversations
            }
            return c;
        }
    }
}
