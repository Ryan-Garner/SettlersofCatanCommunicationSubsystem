using ConversationSubsystem;
using GameManager.AppWorker;
using log4net;
using Messages;
using System;
using System.Collections.Generic;
using System.Net;

namespace GameManager.GameManagerConversations.StartGameConversation
{
    class StartGameStartUpState : ConversationState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartGameStartUpState));

        /// <summary>
        /// Constructor, passes owning conversation to the base class
        /// </summary>
        /// <param name="conv">Conversation that owns this state</param>
        public StartGameStartUpState(StartGameConversation conv)
            : base(conv) { }

        /// <summary>
        /// Sends an initial StartGame message to the user apps and registry and waits for them to respond with an
        /// AckMessage. If they all respond then it will move to the next state, otherwise it will stop the conversation
        /// </summary>
        public override void Execute()
        {
            GameInfo gameInfo = ((GameManagerAppWorker)myConversation.commFacility.myAppWorker).GameInfo;

            List<Envelope> envelopes = new List<Envelope>();

            envelopes.Add(myConversation.FirstEnvelope);
            foreach (IPEndPoint ep in gameInfo.UserEndPoints.Values)
            {
                envelopes.Add(new Envelope(myConversation.FirstEnvelope.message, ep));
            }
            
            Envelope[] receivedEnvelopes = SendAndReceiveMultMessage(envelopes.ToArray());
            
            if (HaveReceivedFromAll(receivedEnvelopes))
            {
                myConversation.SetState(((StartGameConversation)myConversation).GameStateUpdateState);
            }
            else
            {
                myConversation.Stop();
                // TODO: ERROR, didn't receive ACK FROM ALL
            }
        }

        /// <summary>
        /// Checks if the message received is one the state is expecting
        /// </summary>
        /// <param name="env">Message to check</param>
        /// <returns>true if it's an expected message</returns>
        public override bool ReceivedValidMessage(Envelope env)
        {
            return (env?.message?.MessageType == Message.ACK);
        }
    }
}
