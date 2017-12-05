using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConversationSubsystem;
using UserApp;
using Messages;
using System.Net;
using System.Threading;
using UserApp.AppWorker;

namespace GameManagerTest
{
    [TestClass]
    public class EndTurnConversationTest
    {
        public class TestAppWorker : UserAppWorker
        {
            public TestAppWorker()
            {
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
        public void UserApp_EndTurnConversation_Test()
        {
            TestAppWorker testAppWorker = new TestAppWorker();
            testAppWorker.StartTest();

            var fakeGameManager = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };

            fakeGameManager.Start();

            IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Loopback, fakeGameManager.Port);
            Message msg1 = new EndTurnMessage(1, 6);
            Envelope env = new Envelope(msg1, targetEndPoint);

            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            testAppWorker.commFacility.Process(env);

            Assert.IsNotNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            Thread.Sleep(100);

            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);
            Assert.AreEqual(msg1.msgId, _lastIncomingEnvelope1.message.msgId);
            Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);
        }
        private void ProcessEnvelope1(Envelope env)
        {
            _lastIncomingEnvelope1 = env;
        }
    }
}
