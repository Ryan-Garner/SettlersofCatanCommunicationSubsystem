using SharedObjects;
using System.Collections.Concurrent;

namespace ConversationSubsystem
{
    public class ConversationDictionary
    {
        private ConcurrentDictionary<MessageId, Conversation> convDictionary = new ConcurrentDictionary<MessageId, Conversation>();

        /// <summary>
        /// Constructor
        /// </summary>
        public ConversationDictionary()
        {
            convDictionary = new ConcurrentDictionary<MessageId, Conversation>();
        }

        /// <summary>
        /// Attempts to get a conversation via the conversation id of the message passed in
        /// </summary>
        /// <param name="m">Message whose conversation id will be the key</param>
        /// <returns>The conversation if it was successful, otherwise null</returns>
        public Conversation GetConv(MessageId convId)
        {
            Conversation conv;
            if (convDictionary.TryGetValue(convId, out conv))
            {
                return conv;
            }

            return null;
        }

        /// <summary>
        /// Attempts to add the passed conversation to the dictionary with the passed message conversation id as the key
        /// </summary>
        /// <param name="m">Message whose conversation id will be the key</param>
        /// <param name="c">Conversation that will be the value</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool TryAdd(MessageId convId, Conversation c)
        {
            return convDictionary.TryAdd(convId, c);
        }

        /// <summary>
        /// Attempts to remove the conversation from the dictionary with the passed messages conversation id
        /// </summary>
        /// <param name="m">Message whose conversation id will be the key</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Remove(MessageId convId)
        {
            Conversation c;
            return convDictionary.TryRemove(convId, out c);
        }

    }
}
