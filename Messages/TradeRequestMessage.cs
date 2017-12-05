using System.Collections.Generic;
using System.IO;

namespace Messages
{
    public class TradeRequestMessage : Message
    {
        public override short MessageType { get { return TRADE_REQUEST; } }

        public short GameId { get; private set; }
        public short TradeId { get; private set; }
        public short CurrentPlayerId { get; private set; }
        public short PlayerToTradeId { get; private set; }
        public short[] CardsToTrade { get; private set; }

        private TradeRequestMessage()
        {
            GameId = 0;
            TradeId = 0;
            CurrentPlayerId = 0;
            PlayerToTradeId = 0;
            CardsToTrade = null;
        }

        public TradeRequestMessage(short GameId, short TradeId, short CurrentPlayerId, short PlayerToTradeId, short[] CardsToTrade)
            : base()
        {
            this.GameId = GameId;
            this.TradeId = TradeId;
            this.CurrentPlayerId = CurrentPlayerId;
            this.PlayerToTradeId = PlayerToTradeId;
            this.CardsToTrade = CardsToTrade;
        }

        public static TradeRequestMessage Decode(byte[] bytes)
        {
            TradeRequestMessage message = null;
            if (bytes != null)
            {
                message = new TradeRequestMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
                message.TradeId = ReadShort(memoryStream);
                message.CurrentPlayerId = ReadShort(memoryStream);
                message.PlayerToTradeId = ReadShort(memoryStream);
                message.CardsToTrade = ReadShortArray(memoryStream);
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
            Write(memoryStream, CurrentPlayerId);
            Write(memoryStream, PlayerToTradeId);
            Write(memoryStream, CardsToTrade);

            return memoryStream.ToArray();
        }
    }
}
