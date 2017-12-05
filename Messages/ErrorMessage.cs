using SharedObjects;
using System.IO;

namespace Messages
{
    public class ErrorMessage : Message
    {
        public override short MessageType{ get { return ERROR;} }

        public short GameId { get; private set; }
        
        private ErrorMessage()
        {
            GameId = 0;
        }

        public ErrorMessage(MessageId convId, short GameId)
            : base(convId)
        {
            this.GameId = GameId;
        }

        public static ErrorMessage Decode(byte[] bytes)
        {
            ErrorMessage message = null;

            if(bytes != null)
            {
                message = new ErrorMessage();

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