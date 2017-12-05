using System.IO;

namespace Messages
{
    public class StartGameMessage : Message
    {
        //Message type determined by documentation; reflected in filename
        public override short MessageType { get { return START_GAME; } }

        public short GameId { get; private set; }

        private StartGameMessage()
        {
            GameId = 0;
        }

        public StartGameMessage(short GameId)
            : base()
        {
            this.GameId = GameId;
        }

        public static StartGameMessage Decode(byte[] bytes)
        {
            StartGameMessage message = null;
            if (bytes != null)
            {
                message = new StartGameMessage();

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
