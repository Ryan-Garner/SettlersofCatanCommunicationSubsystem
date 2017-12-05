using ConversationSubsystem;
using GameManager.GameManagerConversations.EndTurnConversation;
using GameManager.GameManagerConversations.LaunchConversation;
using GameManager.GameManagerConversations.StartGameConversation;
using GameManager.GameManagerConversations.RollConversation;
using Messages;

namespace GameManager
{
    public class GameManagerConversationFactory : ConversationFactory
    {
        public override Conversation Create(Envelope e)
        {
            Conversation c = null;
            switch(e.message.MessageType)
            {
                case Message.ACK:
                    c = new LaunchConversation(e, ManagingCommFacility);
                    break;
                case Message.START_GAME:
                    c = new StartGameConversation(e, ManagingCommFacility);
                    break;
                case Message.ROLL:
                    c = new RollConversation(e, ManagingCommFacility);
                    break;
                case Message.END_TURN:
                    c = new EndTurnConversation(e, ManagingCommFacility);
                    break;
            }
            return c;
        }
    }
}
