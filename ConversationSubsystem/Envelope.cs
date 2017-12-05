using System.Net;

using Messages;

namespace ConversationSubsystem
{
    public class Envelope
    {
        public Message message;
        public IPEndPoint remoteEndPoint;

        public Envelope(Message message, IPEndPoint remoteEndPoint)
        {
            this.message = message;
            this.remoteEndPoint = remoteEndPoint;
        }
        public bool IsValidToSend => (message != null &&
                                      remoteEndPoint != null &&
                                      remoteEndPoint.Address.ToString() != "0.0.0.0" &&
                                      remoteEndPoint.Port != 0);
    }
}
