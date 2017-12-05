using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using ConversationSubsystem;
using GameManager;
using Messages;
using System.Threading;
using GameManager.GameManagerConversations.StartGameConversation;
using GameManager.AppWorker;

namespace GameManagerTest
{
    [TestClass]
    public class StartGameConversationTest
    {
        public class TestAppWorker : GameManagerAppWorker
        {
            public TestAppWorker(string[] args, GameInfo gameInfo)
                : base(args)
            {
                GameInfo = gameInfo;
                commFacility = new CommFacility(this, new GameManagerConversationFactory());
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
        private Envelope _lastIncomingEnvelope2;
        private Envelope _lastIncomingEnvelope3;
        private Envelope _lastIncomingEnvelope4;
        private Envelope _lastIncomingEnvelope5;

        [TestMethod]
        public void GameManager_StartGameConversation_Test()
        {
            //--------------SET UP VARIABLES-------------------//
            string[] args = { "Test Arguments" };
            GameInfo gameInfo = new GameInfo();
            TestAppWorker testAppWorker = new TestAppWorker(args, gameInfo);
            testAppWorker.StartTest();

            var ua1 = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };

            var ua2 = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope2
            };

            var ua3 = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope3
            };

            var ua4 = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope4
            };

            var registry = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope5
            };

            ua1.Start();
            ua2.Start();
            ua3.Start();
            ua4.Start();
            registry.Start();

            IPEndPoint ua1Ep = new IPEndPoint(IPAddress.Loopback, ua1.Port);
            IPEndPoint ua2Ep = new IPEndPoint(IPAddress.Loopback, ua2.Port);
            IPEndPoint ua3Ep = new IPEndPoint(IPAddress.Loopback, ua3.Port);
            IPEndPoint ua4Ep = new IPEndPoint(IPAddress.Loopback, ua4.Port);
            IPEndPoint registryEp = new IPEndPoint(IPAddress.Loopback, registry.Port);

            Assert.IsTrue(gameInfo.UserEndPoints.TryAdd(0, ua1Ep));
            Assert.IsTrue(gameInfo.UserEndPoints.TryAdd(1, ua2Ep));
            Assert.IsTrue(gameInfo.UserEndPoints.TryAdd(2, ua3Ep));
            Assert.IsTrue(gameInfo.UserEndPoints.TryAdd(3, ua4Ep));

            StartGameMessage msg1 = new StartGameMessage(1);
            Envelope env1 = new Envelope(msg1, registryEp);

            //--------------TEST INITIAL SET UP AND START CONVERSATION-------------------//
            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            testAppWorker.commFacility.Process(env1);

            Thread.Sleep(1000);

            //--------------TEST OUTCOME-------------------//
            Assert.IsTrue(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId) is StartGameConversation);

            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope2);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope3);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope4);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope5);

            // Make sure received messages aren't null
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);

            Assert.IsNotNull(_lastIncomingEnvelope2);
            Assert.IsNotNull(_lastIncomingEnvelope2.message);

            Assert.IsNotNull(_lastIncomingEnvelope3);
            Assert.IsNotNull(_lastIncomingEnvelope3.message);

            Assert.IsNotNull(_lastIncomingEnvelope4);
            Assert.IsNotNull(_lastIncomingEnvelope4.message);

            Assert.IsNotNull(_lastIncomingEnvelope5);
            Assert.IsNotNull(_lastIncomingEnvelope5.message);

            // Make sure received messages are StartGameMessages
            Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);
            StartGameMessage msg2 = _lastIncomingEnvelope1.message as StartGameMessage;
            Assert.IsNotNull(msg2);

            msg2 = _lastIncomingEnvelope2.message as StartGameMessage;
            Assert.IsNotNull(msg2);

            msg2 = _lastIncomingEnvelope3.message as StartGameMessage;
            Assert.IsNotNull(msg2);

            msg2 = _lastIncomingEnvelope4.message as StartGameMessage;
            Assert.IsNotNull(msg2);

            msg2 = _lastIncomingEnvelope5.message as StartGameMessage;
            Assert.IsNotNull(msg2);

            //--------------SEND START ACK MESSAGE-------------------//
            AckMessage msg3 = new AckMessage(msg1.convId, msg1.GameId);
            Envelope env2 = new Envelope(msg3, _lastIncomingEnvelope1.remoteEndPoint);

            _lastIncomingEnvelope1 = null;
            _lastIncomingEnvelope2 = null;
            _lastIncomingEnvelope3 = null;
            _lastIncomingEnvelope4 = null;
            _lastIncomingEnvelope5 = null;

            ua1.Send(env2);
            ua2.Send(env2);
            ua3.Send(env2);
            ua4.Send(env2);
            registry.Send(env2);

            Thread.Sleep(1000);

            //--------------TEST OUTCOME-------------------//
            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope2);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope3);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope4);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope5);

            // Make sure received messages aren't null, except registry's
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);

            Assert.IsNotNull(_lastIncomingEnvelope2);
            Assert.IsNotNull(_lastIncomingEnvelope2.message);

            Assert.IsNotNull(_lastIncomingEnvelope3);
            Assert.IsNotNull(_lastIncomingEnvelope3.message);

            Assert.IsNotNull(_lastIncomingEnvelope4);
            Assert.IsNotNull(_lastIncomingEnvelope4.message);

            Assert.IsNull(_lastIncomingEnvelope5);

            // Make sure received messages are StartGameMessages
            Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);

            GameStateUpdateMessage msg4;

            msg4 = _lastIncomingEnvelope1.message as GameStateUpdateMessage;
            Assert.IsNotNull(msg4);

            msg4 = _lastIncomingEnvelope2.message as GameStateUpdateMessage;
            Assert.IsNotNull(msg4);

            msg4 = _lastIncomingEnvelope3.message as GameStateUpdateMessage;
            Assert.IsNotNull(msg4);

            msg4 = _lastIncomingEnvelope4.message as GameStateUpdateMessage;
            Assert.IsNotNull(msg4);

            //--------------SEND UPDATE ACK MESSAGE-------------------//
            AckMessage msg5 = new AckMessage(msg1.convId, msg1.GameId);
            Envelope env3 = new Envelope(msg5, _lastIncomingEnvelope1.remoteEndPoint);

            _lastIncomingEnvelope1 = null;
            _lastIncomingEnvelope2 = null;
            _lastIncomingEnvelope3 = null;
            _lastIncomingEnvelope4 = null;
            _lastIncomingEnvelope5 = null;

            ua1.Send(env3);
            ua2.Send(env3);
            ua3.Send(env3);
            ua4.Send(env3);

            Thread.Sleep(1000);

            //--------------TEST OUTCOME-------------------//
            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope2);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope3);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope4);
            Assert.AreNotSame(msg1, _lastIncomingEnvelope5);

            // Make sure all received messages are null except ua1's
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);

            Assert.IsNull(_lastIncomingEnvelope2);

            Assert.IsNull(_lastIncomingEnvelope3);

            Assert.IsNull(_lastIncomingEnvelope4);

            Assert.IsNull(_lastIncomingEnvelope5);

            // Check conversation ended
            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            //--------------CLOSE EVERYTHING-------------------//
            testAppWorker.StopTest();
            ua1.Stop();
            ua2.Stop();
            ua3.Stop();
            ua4.Stop();
            registry.Stop();
        }

        private void ProcessEnvelope1(Envelope env)
        {
            _lastIncomingEnvelope1 = env;
        }

        private void ProcessEnvelope2(Envelope env)
        {
            _lastIncomingEnvelope2 = env;
        }

        private void ProcessEnvelope3(Envelope env)
        {
            _lastIncomingEnvelope3 = env;
        }

        private void ProcessEnvelope4(Envelope env)
        {
            _lastIncomingEnvelope4 = env;
        }

        private void ProcessEnvelope5(Envelope env)
        {
            _lastIncomingEnvelope5 = env;
        }
    }
}
