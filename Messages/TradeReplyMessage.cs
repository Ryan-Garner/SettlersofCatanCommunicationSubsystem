using SharedObjects;
using System.IO;

namespace Messages
{
    public class TradeReplyMessage : Message
    {
        public override short MessageType { get { return TRADE_REPLY; } }

        public short GameId { get; private set; }
        public short TradeId { get; private set; }
        public byte Acceptance { get; private set; }

        private TradeReplyMessage()
        {
            GameId = 0;
            TradeId = 0;
            Acceptance = 0;
        }

        public TradeReplyMessage(MessageId convId, short GameId, short TradeId, byte Acceptance)
            : base(convId)
        {
            this.GameId = GameId;
            this.TradeId = TradeId;
            this.Acceptance = Acceptance;
        }

        public static TradeReplyMessage Decode(byte[] bytes)
        {
            TradeReplyMessage message = null;
            if (bytes != null)
            {
                message = new TradeReplyMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
                message.TradeId = ReadShort(memoryStream);
                message.Acceptance = ReadByte(memoryStream);
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
            Write(memoryStream, TradeId);
            Write(memoryStream, Acceptance);

            return memoryStream.ToArray();
        }
    }
}
