using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messages;
using System.Linq;
using SharedObjects;
using System;
using System.Net;

namespace MessagesTest
{
    [TestClass]
    public class TestMessageTypes
    {
        [TestMethod]
        public void TestEncode()
        {
            GameInfoMessage gim = new GameInfoMessage(MessageId.Create(), 1, 2, "1.2.3", "4.5.6");
            byte[] bytes = gim.Encode();

            byte[] messagePIdBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(gim.msgId.Pid));
            byte[] messageSeqBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(gim.msgId.Seq));
            byte[] convPIdBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(gim.convId.Pid));
            byte[] convSeqBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(gim.convId.Seq));

            int currByte = 0;

            //MessageId
            //pid
            Assert.AreEqual(messagePIdBytes[0], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            Assert.AreEqual(messagePIdBytes[1], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            //seq
            Assert.AreEqual(messageSeqBytes[0], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            Assert.AreEqual(messageSeqBytes[1], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");

            //ConversationId
            //pid
            Assert.AreEqual(convPIdBytes[0], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            Assert.AreEqual(convPIdBytes[1], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            //seq
            Assert.AreEqual(convSeqBytes[0], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            Assert.AreEqual(convSeqBytes[1], bytes[currByte], $"bytes[{currByte++}] did not match asserted value");

            //short MessageType
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            Assert.AreEqual(2, bytes[currByte], $"bytes[{currByte++}] did not match asserted value");

            //short GameId
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            Assert.AreEqual(1, bytes[currByte], $"bytes[{currByte++}] did not match asserted value");

            //short UserId
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did not match asserted value");
            Assert.AreEqual(2, bytes[currByte], $"bytes[{currByte++}] did not match asserted value");

            //string GMAddress
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(10, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(49, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(46, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(50, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(46, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(51, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");

            //string UAAddress
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(10, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(52, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(46, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(53, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(46, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(0, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");
            Assert.AreEqual(54, bytes[currByte], $"bytes[{currByte++}] did note match asserted value");

        }

        [TestMethod]
        public void TestDecode()
        {
            byte[] bytes =
            {
                1,2, //message PID
                3,4, //message Seq
                5,6, //conv PID
                7,8, //conv Seq
                0,2, //message type
                0,1, //game id
                0,2, //user id
                0,10,0,49,0,46,0,50,0,46,0,51,//GM address
                0,10,0,52,0,46,0,53,0,46,0,54//UA address
            };

            GameInfoMessage gim = GameInfoMessage.Decode(bytes);

            Assert.AreEqual(gim.GameId, 1);
            Assert.AreEqual(gim.UserId, 2);
            Assert.AreEqual(gim.msgId.Pid, 258);
            Assert.AreEqual(gim.msgId.Seq, 772);
            Assert.AreEqual(gim.convId.Pid, 1286);
            Assert.AreEqual(gim.convId.Seq, 1800);
            Assert.AreEqual(gim.GMAddress, "1.2.3");
            Assert.AreEqual(gim.UAAddress, "4.5.6");

        }

        [TestMethod]
        public void TestDecodeBadByteArray()
        {
            byte[] bytes =
            {
                1,2, //message PID
                3,4, //message Seq
                5,6, //conv PID
                7,8, //conv Seq
                0,2, //message type
                0,1, //game id
                0, //user id missing byte
                0,10,0,49,0,46,0,50,0,46,0,51,//GM address
                0,10,0,52,0,46,0,53,0,46,0,54//UA address
            };
            GameInfoMessage gim = (GameInfoMessage) Message.DecodeMessage(bytes);

            Assert.AreEqual(gim, null, "Attempting to decode a bad byte array did not return null.");

        }

        [TestMethod]
        public void TestAckMessage()
        {
            AckMessage origMessage = new AckMessage(MessageId.Create(), 1);
            byte[] bytes = origMessage.Encode();
            AckMessage decodedMessage = AckMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 11, "Incorrect MessageType");
            Assert.AreEqual(origMessage.msgId, decodedMessage.msgId, "msgId did not match");
            Assert.AreEqual(origMessage.convId, decodedMessage.convId, "convId did not match");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
        }

        [TestMethod]
        public void TestBuyMessage()
        {
            BuyMessage origMessage = new BuyMessage(1, Message.Part.City);
            byte[] bytes = origMessage.Encode();
            BuyMessage decodedMessage = BuyMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 8, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.PartId, decodedMessage.PartId, "PartId did not match");
        }

        [TestMethod]
        public void TestEndGameMessage()
        {
            EndGameMessage origMessage = new EndGameMessage(1);
            byte[] bytes = origMessage.Encode();
            EndGameMessage decodedMessage = EndGameMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 10, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
        }

        [TestMethod]
        public void TestEndTurnMessage()
        {
            EndTurnMessage origMessage = new EndTurnMessage(1,2);
            byte[] bytes = origMessage.Encode();
            EndTurnMessage decodedMessage = EndTurnMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 9, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.CurrentPlayerId, decodedMessage.CurrentPlayerId, "CurrentPlayerId did not match");
        }

        [TestMethod]
        public void TestErrorMessage()
        {
            ErrorMessage origMessage = new ErrorMessage(MessageId.Create(), 1);
            byte[] bytes = origMessage.Encode();
            ErrorMessage decodedMessage = ErrorMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 12, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.msgId, decodedMessage.msgId, "msgId did not match");
            Assert.AreEqual(origMessage.convId, decodedMessage.convId, "convId did not match");
        }

        [TestMethod]
        public void TestGameInfoMessage()
        {
            GameInfoMessage origMessage = new GameInfoMessage(MessageId.Create(), 1, 2, "1.2.3", "4.5.6");
            byte[] bytes = origMessage.Encode();
            GameInfoMessage decodedMessage = GameInfoMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 2, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.msgId, decodedMessage.msgId, "msgId did not match");
            Assert.AreEqual(origMessage.convId, decodedMessage.convId, "convId did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.UserId, decodedMessage.UserId, "UserId did not match");
            Assert.AreEqual(origMessage.GMAddress, decodedMessage.GMAddress, "GMAddress did not match");
            Assert.AreEqual(origMessage.UAAddress, decodedMessage.UAAddress, "UAAddress did not match");
        }

        [TestMethod]
        public void TestGameStateUpdateMessage()
        {
            GameStateUpdateMessage origMessage = new GameStateUpdateMessage(MessageId.Create(), 1, 2, new short[] { 1, 2, 3 }, new short[] { 4, 5, 6 }, new short[] { 7, 8, 9 }, 10, "Macarena");
            byte[] bytes = origMessage.Encode();
            GameStateUpdateMessage decodedMessage = GameStateUpdateMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 4, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.msgId, decodedMessage.msgId, "msgId did not match");
            Assert.AreEqual(origMessage.convId, decodedMessage.convId, "convId did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.CurrentPlayerId, decodedMessage.CurrentPlayerId, "CurrentPlayerId did not match");
            Assert.AreEqual(origMessage.PlayerPoints.Length, decodedMessage.PlayerPoints.Length, "PlayerPoints did not match");
            Assert.IsTrue(!origMessage.PlayerPoints.Except(decodedMessage.PlayerPoints).Any(), "PlayerPoints did not match");
            Assert.AreEqual(origMessage.PlayerCardsAmounts.Length, decodedMessage.PlayerCardsAmounts.Length, "PlayerCardsAmounts did not match");
            Assert.IsTrue(!origMessage.PlayerCardsAmounts.Except(decodedMessage.PlayerCardsAmounts).Any(), "PlayerCardsAmounts did not match");
            Assert.AreEqual(origMessage.BoardLayout.Length, decodedMessage.BoardLayout.Length, "BoardLayout did not match");
            Assert.IsTrue(!origMessage.BoardLayout.Except(decodedMessage.BoardLayout).Any(), "BoardLayout did not match");
            Assert.AreEqual(origMessage.PlayerWithLR, decodedMessage.PlayerWithLR, "PlayerWithLR did not match");
            Assert.AreEqual(origMessage.LastMove, decodedMessage.LastMove, "LastMove did not match");
        }

        [TestMethod]
        public void TestRequestGameMessage()
        {
            RequestGameMessage origMessage = new RequestGameMessage();
            byte[] bytes = origMessage.Encode();
            RequestGameMessage decodedMessage = RequestGameMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 1, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
        }

        [TestMethod]
        public void TestRollMessage()
        {
            RollMessage origMessage = new RollMessage(1,2);
            byte[] bytes = origMessage.Encode();
            RollMessage decodedMessage = RollMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 5, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.Roll, decodedMessage.Roll, "Roll did not match");
        }

        [TestMethod]
        public void TestStartGameMessage()
        {
            StartGameMessage origMessage = new StartGameMessage(1);
            byte[] bytes = origMessage.Encode();
            StartGameMessage decodedMessage = StartGameMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 3, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
        }

        [TestMethod]
        public void TestTradeReplyMessage()
        {
            TradeReplyMessage origMessage = new TradeReplyMessage(MessageId.Create(), 1, 2,0);
            byte[] bytes = origMessage.Encode();
            TradeReplyMessage decodedMessage = TradeReplyMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 7, "Incorrect MessageType");
            Assert.AreEqual(origMessage.msgId, decodedMessage.msgId, "msgId did not match");
            Assert.AreEqual(origMessage.convId, decodedMessage.convId, "convId did not match");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.TradeId, decodedMessage.TradeId, "TradeId did not match");
            Assert.AreEqual(origMessage.Acceptance, decodedMessage.Acceptance, "Acceptance did not match");
        }

        [TestMethod]
        public void TestTradeRequestMessage()
        {
            short[] CardsToTrade = { 1, 2, 3 };
            TradeRequestMessage origMessage = new TradeRequestMessage(1,2,3,4,CardsToTrade);
            byte[] bytes = origMessage.Encode();
            TradeRequestMessage decodedMessage = TradeRequestMessage.Decode(bytes);

            Assert.AreEqual(origMessage.MessageType, 6, "Incorrect MessageType");
            Assert.AreEqual(origMessage.MessageType, decodedMessage.MessageType, "MessageType did not match");
            Assert.AreEqual(origMessage.GameId, decodedMessage.GameId, "GameId did not match");
            Assert.AreEqual(origMessage.TradeId, decodedMessage.TradeId, "TradeId did not match");
            Assert.AreEqual(origMessage.CurrentPlayerId, decodedMessage.CurrentPlayerId, "CurrentPlayerId did not match");
            Assert.AreEqual(origMessage.PlayerToTradeId, decodedMessage.PlayerToTradeId, "PlayerToTradeId did not match");
            Assert.AreEqual(origMessage.CardsToTrade.Length, decodedMessage.CardsToTrade.Length, "CardsToTrade did not match");
            Assert.IsTrue(!origMessage.CardsToTrade.Except(decodedMessage.CardsToTrade).Any(), "CardsToTrade did not match");
        }
    }
}
