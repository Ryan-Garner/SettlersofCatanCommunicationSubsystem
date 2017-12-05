using System.IO;

namespace Messages
{
    public class RollMessage : Message
    {
        //Message type determined by documentation; reflected in filename
        public override short MessageType { get { return ROLL; } }

        public short GameId { get; private set; }
        public short Roll { get; private set; }

        private RollMessage()
        {
            GameId = 0;
            Roll = 0;
        }

        public RollMessage(short GameId, short Roll)
            : base()
        {
            this.GameId = GameId;
            this.Roll = Roll;
        }

        public static RollMessage Decode(byte[] bytes)
        {
            RollMessage message = null;
            if (bytes != null)
            {
                message = new RollMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
                message.Roll = ReadShort(memoryStream);
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
            Write(memoryStream, Roll);

            return memoryStream.ToArray();
        }
    }
}
