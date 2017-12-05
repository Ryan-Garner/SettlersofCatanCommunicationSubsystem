using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameManager;
using GameManager.AppWorker;
using GameManager.AppWorker.States;
using ConversationSubsystem;
using System.Net;
using Messages;
using System.Threading;

namespace GameManagerTest
{
    [TestClass]
    class RollConversationTest
    {
        public class TestAppWorker : GameManagerAppWorker
        {
            public TestAppWorker(string[] testArgs)
                : base(testArgs)
            {
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
        private string[] testArgs = { "1", "2" };

        [TestMethod]
        public void UserApp_RollConversation_Test()
        {
            TestAppWorker testAppWorker = new TestAppWorker(testArgs);
            testAppWorker.SetState(new TempState(testAppWorker));
            testAppWorker.StartTest();

            var fakeUserApp = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };

            fakeUserApp.Start();



            IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Loopback, fakeUserApp.Port);
            Message msg1 = new RollMessage(1, 6);
            Envelope env = new Envelope(msg1, targetEndPoint);

            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            testAppWorker.commFacility.DelegateToConversation(env);

            Assert.IsNotNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            Thread.Sleep(100);

            /* Assert.AreNotSame(msg1, _lastIncomingEnvelope1);
             Assert.IsNotNull(_lastIncomingEnvelope1);
             Assert.IsNotNull(_lastIncomingEnvelope1.message);
             Assert.AreEqual(msg1.msgId, _lastIncomingEnvelope1.message.msgId);
             Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);*/
        }
        private void ProcessEnvelope1(Envelope env)
        {
            _lastIncomingEnvelope1 = env;
        }
    }
}
