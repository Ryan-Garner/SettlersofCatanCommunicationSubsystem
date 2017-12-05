using SharedObjects;
using System.IO;

namespace Messages
{
    public class GameStateUpdateMessage : Message
    {
        public override short MessageType { get { return GAME_STATE_UPDATE; } }

        public short GameId { get; private set; }
        public short CurrentPlayerId { get; private set; }
        // TODO: Make DeckType
        // public DeckType AvailableResources;
        public short[] PlayerPoints { get; private set; }
        public short[] PlayerCardsAmounts { get; private set; }
        public short[] BoardLayout { get; private set; }
        public short PlayerWithLR { get; private set; }
        public string LastMove { get; private set; }


        private GameStateUpdateMessage()
        {
            GameId = 0;
            CurrentPlayerId = 0;
            // TODO: Initialize AvailableResources with a DeckType
            PlayerPoints = null;
            PlayerCardsAmounts = null;
            BoardLayout = null;
            PlayerWithLR = 0;
            LastMove = string.Empty;
        }

        // TODO: Add AvailableResources : DeckType as a parameter
        public GameStateUpdateMessage(MessageId convId, short GameId, short CurrentPlayerId, short[] PlayerPoints, short[] PlayerCardsAmounts, short[] BoardLayout, short PlayerWithLR, string LastMove)
            : base(convId)
        {
            this.GameId = GameId;
            this.CurrentPlayerId = CurrentPlayerId;
            // TODO: Set AvailableResources with the passed in DeckType value
            this.PlayerPoints = PlayerPoints;
            this.PlayerCardsAmounts = PlayerCardsAmounts;
            this.BoardLayout = BoardLayout;
            this.PlayerWithLR = PlayerWithLR;
            this.LastMove = LastMove;
        }

        public static GameStateUpdateMessage Decode(byte[] bytes)
        {
            GameStateUpdateMessage message = null;
            if (bytes != null)
            {
                message = new GameStateUpdateMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
                message.CurrentPlayerId = ReadShort(memoryStream);
                // TODO: ReadShort to AvailableResources
                message.PlayerPoints = ReadShortArray(memoryStream);
                message.PlayerCardsAmounts = ReadShortArray(memoryStream);
                message.BoardLayout = ReadShortArray(memoryStream);
                message.PlayerWithLR = ReadShort(memoryStream);
                message.LastMove = ReadString(memoryStream);
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
            // TODO: Write AvailableResources
            Write(memoryStream, CurrentPlayerId);
            Write(memoryStream, PlayerPoints);
            Write(memoryStream, PlayerCardsAmounts);
            Write(memoryStream, BoardLayout);
            Write(memoryStream, PlayerWithLR);
            Write(memoryStream, LastMove);

            return memoryStream.ToArray();
        }
    }
}
