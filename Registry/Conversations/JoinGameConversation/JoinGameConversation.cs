using ConversationSubsystem;
using SharedObjects;
using System.Net;
using Messages;

namespace Registry.Conversations.JoinGameConversation
{
    public class JoinGameConversation : Conversation
    {
        public ConversationState StartUpState { get; private set; }
        public ConversationState WaitForGameManagerState { get; private set; }

        public JoinGameConversation(Envelope envelope, CommFacility commSubsystem) 
            : base(envelope, commSubsystem)
        {
            
            StartUpState = new JoinGameStartUpState(this);
            WaitForGameManagerState = new JoinGameWaitForGameManagerState(this);
            currentState = StartUpState;
        }

        public void SendGameInfoMessage(RegistryData.GameInfo gameInfo)
        {
            MessageId currentConvId = FirstEnvelope.message.convId;
            IPEndPoint userEndPoint = FirstEnvelope.remoteEndPoint;
            gameInfo.Players++;
            Message msg = new GameInfoMessage(currentConvId, gameInfo.GameId, gameInfo.Players, gameInfo.RemoteEndPoint.ToString(), userEndPoint.ToString());
            Envelope userEnv = new Envelope(msg, userEndPoint);
            Envelope gameManagerEnv = new Envelope(msg, gameInfo.RemoteEndPoint);
            commFacility.Send(userEnv);
            commFacility.Send(gameManagerEnv);
        }
    }
}
