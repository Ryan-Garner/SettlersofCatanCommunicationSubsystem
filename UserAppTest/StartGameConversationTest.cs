using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserApp.AppWorker;
using UserApp;
using ConversationSubsystem;
using System.Net;
using Messages;
using System.Threading;
using UserApp.UserAppConversations.StartGameConversation;

namespace UserAppTest
{
    [TestClass]
    public class StartGameConversationTest
    {
        public class TestAppWorker : UserAppWorker
        {
            public TestAppWorker(UserInfo userInfo)
                : base()
            {
                UserInfo = userInfo;
                commFacility = new CommFacility(this, new UserAppConversationFactory());
            }

            public void StartTest()
            {
                commFacility.Initialize();
                commFacility.Start();
            }

            public void StopTest()
            {
                commFacility.Stop();
            }
        }

        private Envelope _lastIncomingEnvelope1;

        [TestMethod]
        public void UserApp_StartGameConversation_Test()
        {
            //--------------SET UP VARIABLES-------------------//
            UserInfo userInfo = new UserInfo()
            {
                PlayerId = 0,
                CurrentPlayerId = -1,
                AcceptUserInput = false
            };

            TestAppWorker testAppWorker = new TestAppWorker(userInfo);
            testAppWorker.StartTest();

            var gameManager = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };
            gameManager.Start();

            IPEndPoint userAppEp = new IPEndPoint(IPAddress.Loopback, testAppWorker.commFacility.udpCommunicator.Port);

            testAppWorker.UserInfo.GameManagerEP = new IPEndPoint(IPAddress.Loopback, gameManager.Port);

            StartGameMessage msg1 = new StartGameMessage(1);
            Envelope env1 = new Envelope(msg1, userAppEp);

            //--------------TEST INITIAL SET UP AND SEND INITIAL MESSAGE-------------------//
            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            gameManager.Send(env1);

            Thread.Sleep(1000);

            //--------------TEST OUTCOME-------------------//
            Assert.IsTrue(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId) is StartGameConversation);

            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);

            // Make sure received message isn't null
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);

            // Make sure received message is AckMessage
            Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);
            AckMessage msg2 = _lastIncomingEnvelope1.message as AckMessage;
            Assert.IsNotNull(msg2);

            //--------------SEND START GAME STATE UPDATE MESSAGE-------------------//
            GameStateUpdateMessage msg3 = new GameStateUpdateMessage(msg1.convId, 1, 0, new short[4], new short[4], new short[84], 0, "TEST APP MOVE");
            Envelope env2 = new Envelope(msg3, _lastIncomingEnvelope1.remoteEndPoint);

            _lastIncomingEnvelope1 = null;

            // User should not know it is their turn
            Assert.IsFalse(userInfo.IsTurn);

            gameManager.Send(env2);

            Thread.Sleep(1000);

            //--------------TEST OUTCOME-------------------//
            // User should know it is their turn
            Assert.IsTrue(userInfo.IsTurn);

            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);

            // Make sure received message isn't null
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);

            // Make sure received message is AckMessage
            Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);

            AckMessage msg4 = _lastIncomingEnvelope1.message as AckMessage;

            Assert.IsNotNull(msg4);

            //--------------SEND FIRST PLAYER GO ACK MESSAGE-------------------//
            AckMessage msg5 = new AckMessage(msg1.convId, msg1.GameId);
            Envelope env3 = new Envelope(msg5, _lastIncomingEnvelope1.remoteEndPoint);

            _lastIncomingEnvelope1 = null;

            // Check user isn't able to input
            Assert.IsFalse(userInfo.AcceptUserInput);

            gameManager.Send(env3);

            Thread.Sleep(1000);

            //--------------TEST OUTCOME-------------------//
            // Check user is able to input
            Assert.IsTrue(userInfo.AcceptUserInput);

            // Check conversation ended
            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            //--------------CLOSE EVERYTHING-------------------//
            testAppWorker.StopTest();
            gameManager.Stop();
        }

        private void ProcessEnvelope1(Envelope env)
        {
            _lastIncomingEnvelope1 = env;
        }
    }
}
