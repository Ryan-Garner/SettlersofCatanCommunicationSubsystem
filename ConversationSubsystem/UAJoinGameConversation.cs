using System;
using System.Threading;

using Messages;


namespace ConversationSubsystem
{
    public class UAJoinGameConversation : Conversation
    {
        private Thread convThread;
        private bool keepGoing = false;

        public UAJoinGameConversation(Envelope envelope)
            :base(envelope)
        {
            convThread = new Thread(new ThreadStart(Communicate));
            keepGoing = true;
        }
        
        public override void Start()
        {
            convThread.Start();
        }

        private void Communicate()
        {
            while (keepGoing)
            {
                if (envelopeQueue.Count > 0)
                {
                    for (int i = 0; i > envelopeQueue.Count; i++)
                    {
                        envelopeQueue.TryDequeue(out Envelope e);
                        if (e.message.msgId != firstEnvelope.message.msgId)
                        {
                            if(e.message.MessageType == 2)
                            {
                                ReceivedGameInfo(e.message);
                            }
                        }
                    }
                }
            }
        }

        private void ReceivedGameInfo(Message message)
        {
            // TODO: UpdateGameINFO
            Stop();
        }

        public override void Stop()
        {
            keepGoing = false;
            convThread.Abort();
        }
    }
}
