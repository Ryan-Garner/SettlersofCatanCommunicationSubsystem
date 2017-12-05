using SharedObjects;
using System.IO;

namespace Messages
{
    public class GameInfoMessage : Message
    {
        //Message type determined by documentation; reflected in filename
        public override short MessageType { get { return GAME_INFO; } }

        public short GameId { get; private set; }
        public short UserId { get; private set; }
        public string GMAddress { get; private set; }
        public string UAAddress { get; private set; }

        private GameInfoMessage()
        {
            GameId = 0;
            UserId = 0;
            GMAddress = string.Empty;
            UAAddress = string.Empty;
        }

        public GameInfoMessage(MessageId convId, short GameId, short UserId, string GMAddress, string UAAddress)
            : base(convId)
        {
            this.GameId = GameId;
            this.UserId = UserId;
            this.GMAddress = GMAddress;
            this.UAAddress = UAAddress;
        }

        public static GameInfoMessage Decode(byte[] bytes)
        {
            GameInfoMessage message = null;
            if (bytes != null)
            {
                message = new GameInfoMessage();

                MemoryStream memoryStream = new MemoryStream(bytes);

                message.msgId = ReadMessageId(memoryStream);
                message.convId = ReadMessageId(memoryStream);
                ReadShort(memoryStream);
                message.GameId = ReadShort(memoryStream);
                message.UserId = ReadShort(memoryStream);
                message.GMAddress = ReadString(memoryStream);
                message.UAAddress = ReadString(memoryStream);
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
            Write(memoryStream, UserId);
            Write(memoryStream, GMAddress);
            Write(memoryStream, UAAddress);

            return memoryStream.ToArray();
        }
    }
}
