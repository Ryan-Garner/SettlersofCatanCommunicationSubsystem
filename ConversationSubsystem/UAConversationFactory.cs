using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversationSubsystem
{
    public class UAConversationFactory : ConversationFactory
    {
        public void CreateConversation(Envelope e)
        {
            Conversation conversation;
            if (e.message.MessageType == 1)
            {
                conversation = new UAJoinGameConversation(e);
                ConversationDictionary.TryAdd(e.message.convId, conversation);
            }
            // TODO: Add the rest of the conversations;
        }
    }
}
