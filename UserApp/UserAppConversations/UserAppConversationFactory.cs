using ConversationSubsystem;
using UserApp.UserAppConversations.JoinGameConversation;
using UserApp.UserAppConversations.StartGameConversation;
using UserApp.UserAppConversations.RollConversation;
using Messages;
using UserApp.UserAppConversations.EndTurnConversation;

namespace UserApp
{
    public class UserAppConversationFactory : ConversationFactory
    {
        /// <summary>
        /// Creates and returns a conversation based on the envelope passed in
        /// </summary>
        /// <param name="e">Envelope with message to create conversation with</param>
        /// <returns>Created conversation if valid message, else null</returns>
        public override Conversation Create(Envelope e)
        {
            Conversation c = null;
            switch(e.message.MessageType)
            {
                case Message.REQUEST_GAME:
                    c = new JoinGameConversation(e, ManagingCommFacility);
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
               //TODO: Add cases for the other conversations
            }
            return c;
        }

    }
}
