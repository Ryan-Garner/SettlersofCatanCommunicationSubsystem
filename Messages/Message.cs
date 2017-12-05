using System;
using System.IO;
using System.Net;
using System.Text;

using SharedObjects;

namespace Messages
{
    public abstract class Message
    {
        public enum Part
        {
            Settlement = 0,
            Road = 1,
            City = 2
        }

        public const short REQUEST_GAME = 1;
        public const short GAME_INFO = 2;
        public const short START_GAME = 3;
        public const short GAME_STATE_UPDATE = 4;
        public const short ROLL = 5;
        public const short TRADE_REQUEST = 6;
        public const short TRADE_REPLY = 7;
        public const short BUY = 8;
        public const short END_TURN = 9;
        public const short END_GAME = 10;
        public const short ACK = 11;
        public const short ERROR = 12;

        //Message types determined by documentation; reflected in filenames
        public abstract short MessageType { get; }

        public MessageId msgId;
        public MessageId convId;

        public Message()
        {
            msgId = MessageId.Create();
            convId = msgId.Clone();
        }

        public Message(MessageId convId)
        {
            msgId = MessageId.Create();
            this.convId = convId.Clone();
        }

        public abstract byte[] Encode();

        public static Message DecodeMessage(byte[] messageByteStream)
        {
            MemoryStream memoryStream = new MemoryStream(messageByteStream);
            Message message = null;

            ReadMessageId(memoryStream);
            ReadMessageId(memoryStream);

            short messageType = ReadShort(memoryStream);
            try
            {
                switch (messageType)
                {
                    case REQUEST_GAME:
                        message = RequestGameMessage.Decode(messageByteStream);
                        break;
                    case GAME_INFO:
                        message = GameInfoMessage.Decode(messageByteStream);
                        break;
                    case START_GAME:
                        message = StartGameMessage.Decode(messageByteStream);
                        break;
                    case GAME_STATE_UPDATE:
                        message = GameStateUpdateMessage.Decode(messageByteStream);
                        break;
                    case ROLL:
                        message = RollMessage.Decode(messageByteStream);
                        break;
                    case TRADE_REQUEST:
                        message = TradeRequestMessage.Decode(messageByteStream);
                        break;
                    case TRADE_REPLY:
                        message = TradeReplyMessage.Decode(messageByteStream);
                        break;
                    case BUY:
                        message = BuyMessage.Decode(messageByteStream);
                        break;
                    case END_TURN:
                        message = EndTurnMessage.Decode(messageByteStream);
                        break;
                    case END_GAME:
                        message = EndGameMessage.Decode(messageByteStream);
                        break;
                    case ACK:
                        message = AckMessage.Decode(messageByteStream);
                        break;
                    case ERROR:
                        message = ErrorMessage.Decode(messageByteStream);
                        break;
                    default:

                        break;
                }

                return message;
            }
            catch
            {
                return null;
            }
        }

        protected static void Write(MemoryStream memoryStream, short value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            memoryStream.Write(bytes, 0, bytes.Length);
        }

        protected static void Write(MemoryStream memoryStream, short[] values)
        {
            Write(memoryStream, (short)values.Length);

            for (int i = 0; i < values.Length; ++i)
            {
                Write(memoryStream, values[i]);
            }
        }

        protected static void Write(MemoryStream memoryStream, byte value)
        {
            memoryStream.WriteByte(value);
        }

        protected static void Write(MemoryStream memoryStream, int value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            memoryStream.Write(bytes, 0, bytes.Length);
        }

        protected static void Write(MemoryStream memoryStream, long value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            memoryStream.Write(bytes, 0, bytes.Length);
        }

        protected static void Write(MemoryStream memoryStream, string value)
        {
            byte[] bytes = Encoding.BigEndianUnicode.GetBytes(value);
            Write(memoryStream, (short)bytes.Length);
            memoryStream.Write(bytes, 0, bytes.Length);
        }

        protected static void Write(MemoryStream memoryStream, MessageId mId)
        {
            Write(memoryStream, mId.Pid);
            Write(memoryStream, mId.Seq);
        }

        public static short ReadShort(byte[] bytes)
        {
            MemoryStream memoryStream = new MemoryStream(bytes);

            return ReadShort(memoryStream);
        }

        protected static short ReadShort(MemoryStream memoryStream)
        {
            byte[] bytes = new byte[2];
            int bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length)
                throw new ApplicationException("Cannot decode a short integer from message");

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 0));
        }

        protected static short[] ReadShortArray(MemoryStream memoryStream)
        {
            int size = ReadShort(memoryStream);

            short[] shorts = new short[size];

            for (int i = 0; i < size; ++i)
            {
                shorts[i] = ReadShort(memoryStream);
            }

            return shorts;
        }

        protected static int ReadInt(MemoryStream memoryStream)
        {
            byte[] bytes = new byte[4];
            int bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length)
                throw new ApplicationException("Cannot decode an integer from message");

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
        }

        protected static long ReadLong(MemoryStream memoryStream)
        {
            byte[] bytes = new byte[8];
            int bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length)
                throw new ApplicationException("Cannot decode a long integer from message");

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(bytes, 0));
        }

        protected static string ReadString(MemoryStream memoryStream)
        {
            string result = String.Empty;
            int length = ReadShort(memoryStream);
            if (length > 0)
            {
                byte[] bytes = new byte[length];
                int bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
                if (bytesRead != length)
                    throw new ApplicationException("Cannot decode a string from message");

                result = Encoding.BigEndianUnicode.GetString(bytes, 0, bytes.Length);
            }
            return result;
        }

        protected static byte ReadByte(MemoryStream memoryStream)
        {
            byte[] bite = new byte[1];
            int bytesRead = memoryStream.Read(bite, 0, 1);
            if (bytesRead != bite.Length)
                throw new ApplicationException("Cannot decode single byte from message");

            return bite[0];

        }

        protected static MessageId ReadMessageId(MemoryStream memoryStream)
        {
            MessageId mId = new MessageId
            {
                Pid = ReadShort(memoryStream),
                Seq = ReadShort(memoryStream)
            };

            return mId;
        }
    }
}
