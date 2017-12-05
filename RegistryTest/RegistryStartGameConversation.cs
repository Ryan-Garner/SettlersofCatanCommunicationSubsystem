using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Registry.AppWorker;
using Registry;
using ConversationSubsystem;
using Registry.Conversations;
using System.Net;
using Messages;

namespace RegistryTest
{
    [TestClass]
    public class StartGameConversationTest
    {
        public class TestAppWorker : RegistryAppWorker
        {
            public TestAppWorker(RegistryData registryData)
                : base()
            {
                RegistryData = registryData;
                commFacility = new CommFacility(this, new RegistryConversationFactory());
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
            RegistryData registryData = new RegistryData();
            RegistryData.GameInfo gameInfo = new RegistryData.GameInfo();

            TestAppWorker testAppWorker = new TestAppWorker(registryData);
            testAppWorker.StartTest();

            var gameManager = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };
            gameManager.Start();

            IPEndPoint registryEp = new IPEndPoint(IPAddress.Loopback, testAppWorker.commFacility.udpCommunicator.Port);
            IPEndPoint gameManagerEp = new IPEndPoint(IPAddress.Loopback, gameManager.Port);
            gameInfo.RemoteEndPoint = gameManagerEp;
            gameInfo.GameActive = false;

            Assert.IsTrue(registryData.AddGame(gameInfo));

            StartGameMessage msg1 = new StartGameMessage(1);
            Envelope env1 = new Envelope(msg1, registryEp);

            //--------------TEST INITIAL SET UP AND SEND INITIAL MESSAGE-------------------//
            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));
            Assert.IsFalse(gameInfo.GameActive);

            gameManager.Send(env1);

            Thread.Sleep(1000);

            //--------------TEST OUTCOME-------------------//
            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);

            // Make sure received message isn't null
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);

            // Make sure received message is AckMessage
            Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);
            AckMessage msg2 = _lastIncomingEnvelope1.message as AckMessage;
            Assert.IsNotNull(msg2);

            Assert.IsTrue(gameInfo.GameActive);
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
