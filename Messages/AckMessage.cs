using SharedObjects;
using System.IO;

namespace Messages
{
    public class AckMessage : Message
    {
        public override short MessageType{ get { return ACK;} }

        public short GameId { get; private set; }
        
        private AckMessage()
        {
            GameId = 0;
        }

        public AckMessage(MessageId convId, short GameId)
            : base(convId)
        {
            this.GameId = GameId;
        }

        public static AckMessage Decode(byte[] bytes)
        {
            AckMessage message = null;

            if(bytes != null)
            {
                message = new AckMessage();

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