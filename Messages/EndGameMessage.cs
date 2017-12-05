using System.IO;

namespace Messages
{
    public class EndGameMessage : Message
    {
        public override short MessageType{ get { return END_GAME;} }

        public short GameId { get; private set; }
        
        private EndGameMessage()
        {
            GameId = 0;
        }

        public EndGameMessage(short GameId)
            : base()
        {
            this.GameId = GameId;
        }

        public static EndGameMessage Decode(byte[] bytes)
        {
            EndGameMessage message = null;

            if(bytes != null)
            {
                message = new EndGameMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
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

            return memoryStream.ToArray();
        }
    }
}