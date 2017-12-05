using System.IO;

namespace Messages
{
    public class BuyMessage : Message
    {
        public override short MessageType { get { return BUY; } }

        public short GameId { get; private set; }
        public Part PartId { get; private set; }

        private BuyMessage()
        {
            GameId = 0;
            PartId = Part.Road;
        }

        public BuyMessage(short GameId, Part PartId)
            : base()
        {
            this.GameId = GameId;
            this.PartId = PartId;
        }

        public static BuyMessage Decode(byte[] bytes)
        {
            BuyMessage message = null;
            if (bytes != null)
            {
                message = new BuyMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
                message.PartId = (Part)ReadShort(memoryStream);
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
            Write(memoryStream, (short)PartId);

            return memoryStream.ToArray();
        }
    }
}
