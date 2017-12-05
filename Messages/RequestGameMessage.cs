using System.IO;

namespace Messages
{
    public class RequestGameMessage : Message
    {
        //Message type determined by documentation; reflected in filename
        public override short MessageType { get { return REQUEST_GAME; } }

        public RequestGameMessage()
            : base() { }

        public static RequestGameMessage Decode(byte[] bytes)
        {
            RequestGameMessage message = null;
            if(bytes != null)
            {
                message = new RequestGameMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
            }

            return message;
        }

        public override byte[] Encode()
        {
            MemoryStream memoryStream = new MemoryStream();

            Write(memoryStream, msgId);
            Write(memoryStream, convId);
            Write(memoryStream, MessageType);

            return memoryStream.ToArray();
        }
    }
}
