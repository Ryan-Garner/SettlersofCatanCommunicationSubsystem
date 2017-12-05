using SharedObjects;
using System;
using System.Collections.Generic;

namespace ConversationSubsystem
{
    public abstract class ConversationState
    {
        private const int RECEIVE_TIMEOUT = 100;
        private const int MAX_WAITS = 30;
        private const int MAX_SENDS = 3;
        protected Conversation myConversation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">Conversation state belongs to</param>
        public ConversationState(Conversation c)
        {
            myConversation = c;
        }
        
        public abstract void Execute();
        public virtual bool ReceivedValidMessage(Envelope env) { throw new NotImplementedException(); }

        /// <summary>
        /// Sends message and waits for a response
        /// </summary>
        /// <param name="env">Envelope to send</param>
        /// <returns>Message received, else null</returns>
        public Envelope SendAndReceiveMessage(Envelope env)
        {
            Envelope receivedMessage = null;

            bool validResponseReceived = false;

            int sendsRemaining = MAX_SENDS;

            while (sendsRemaining > 0 && !validResponseReceived)
            {
                SendMessage(env);
                sendsRemaining--;
                receivedMessage = ReceiveMessage();
                if (ReceivedValidMessage(receivedMessage))
                {
                    return receivedMessage;
                }
            }

            return null;
        }

        /// <summary>
        /// Sends messages and waits for all those sent to, to respond
        /// </summary>
        /// <param name="env">Envelopes to send</param>
        /// <returns>Envelopes received from where messages were sent, null in the slots no messages were recieved</returns>
        public Envelope[] SendAndReceiveMultMessage(Envelope[] envelopes)
        {
            Envelope[] tempReceivedMessages = null;
            Envelope[] receivedMessages = new Envelope[envelopes.Length];

            int sendsRemaining = MAX_SENDS;

            while (sendsRemaining > 0 && !HaveReceivedFromAll(receivedMessages))
            {
                for (int i = 0; i < envelopes.Length; ++i)
                {
                    if (receivedMessages[i] == null) {
                        SendMessage(envelopes[i]);
                    }
                }

                sendsRemaining--;
                tempReceivedMessages = ReceiveMultMessage();

                foreach (Envelope envReceived in tempReceivedMessages)
                {
                    if (ReceivedValidMessage(envReceived))
                    {
                        for (int i = 0; i < envelopes.Length; ++i)
                        {
                            if (envelopes[i].remoteEndPoint.Equals(envReceived.remoteEndPoint))
                            {
                                receivedMessages[i] = envReceived;
                            }
                        }
                    }
                }
            }

            return receivedMessages;
        }

        /// <summary>
        /// Checks if everyone expected to send a message did
        /// </summary>
        /// <param name="envelopes">Envelopes of received messages to check</param>
        /// <returns>false if any envelope is null, else true</returns>
        protected bool HaveReceivedFromAll(Envelope[] envelopes)
        {
            foreach (Envelope env in envelopes)
            {
                if (env == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sends message through the conversation's commFacility
        /// </summary>
        /// <param name="env">Envelope to send</param>
        /// <returns>Error that is returned from commFacility's Send()</returns>
        public Error SendMessage(Envelope env)
        {
            return myConversation.commFacility.Send(env);
        }

        /// <summary>
        /// Thread sleeps for RECEIVE_TIMEOUT ms then checks the queue for a message
        /// </summary>
        /// <returns>Last envelope received, else null</returns>
        public Envelope ReceiveMessage()
        {
            Envelope envReceived = null;
            Envelope lastEnvReceived = null;

            int waits = MAX_WAITS;

            while (lastEnvReceived == null && waits > 0)
            {
                waits--;
                while (myConversation.envelopeQueue.TryTake(out envReceived, RECEIVE_TIMEOUT))
                {
                    lastEnvReceived = envReceived;
                }
            }

            return lastEnvReceived;
        }

        /// <summary>
        /// Thread sleeps for RECEIVE_TIMEOUT ms then checks the queue for a any received messages and returns them
        /// </summary>
        /// <returns>Envelopes received, else null</returns>
        public Envelope[] ReceiveMultMessage()
        {
            List<Envelope> envelopesReceived = new List<Envelope>();
            Envelope envReceived = null;

            int waits = MAX_WAITS;

            while (envelopesReceived.Count == 0 && waits > 0) {
                waits--;
                while (myConversation.envelopeQueue.TryTake(out envReceived, RECEIVE_TIMEOUT))
                {
                    envelopesReceived.Add(envReceived);
                }
            }

            return envelopesReceived.ToArray();
        }
    }
}
