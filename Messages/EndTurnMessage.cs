using System.IO;

namespace Messages
{
    public class EndTurnMessage : Message
    {
        public override short MessageType { get {return END_TURN;} }

        public short GameId { get; private set; }
        public short CurrentPlayerId { get; private set; }

        private EndTurnMessage()
        {
            GameId = 0;
            CurrentPlayerId = 0;
        }

        public EndTurnMessage(short GameId, short CurrentPlayerId)
            : base()
        {
            this.GameId = GameId;
            this.CurrentPlayerId = CurrentPlayerId;
        }

        public static EndTurnMessage Decode(byte[] bytes)
        {
            EndTurnMessage message = null;
            if(bytes != null)
            {
                message = new EndTurnMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
                message.CurrentPlayerId = ReadShort(memoryStream);
            }
            return message;
        }

        public override byte[] Encode()
        {
            MemoryStream memoryStream = new MemoryStream();

            Write(memoryStream, msgId);
            Write(memoryStream, convId);
            Write(memoryStream, MessageType);
            Write(memoryStream, GameId);
            Write(memoryStream, CurrentPlayerId);

            return memoryStream.ToArray();
        }
    }
}