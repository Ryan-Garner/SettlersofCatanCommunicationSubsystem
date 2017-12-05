using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConversationSubsystem;
using UserApp;
using Messages;
using System.Net;
using System.Threading;
using SharedObjects;
using UserApp.AppWorker;

namespace UserAppTest
{
    [TestClass]
    public class JoinGameConversationTest
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
        public void UserApp_JoinGameConversation_Test()
        {
            TestAppWorker testAppWorker = new TestAppWorker();
            testAppWorker.StartTest();

            var fakeRegistry = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };
            fakeRegistry.Start();

            IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Loopback, fakeRegistry.Port);

            Message msg1 = new RequestGameMessage();
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
            RequestGameMessage msg2 = _lastIncomingEnvelope1.message as RequestGameMessage;
            Assert.IsNotNull(msg2);

            GameInfoMessage msg3 = new GameInfoMessage(msg1.convId, 1, 1, "GMAddress", "UAAddress");

            Assert.AreNotSame((IPEndPoint)fakeRegistry._myUdpClient.Client.LocalEndPoint, _lastIncomingEnvelope1.remoteEndPoint);

            Envelope env2 = new Envelope(msg3, _lastIncomingEnvelope1.remoteEndPoint);
            fakeRegistry.Send(env2);

            Thread.Sleep(100);

            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            fakeRegistry.Stop();
            testAppWorker.StopTest();
            
        }

        private void ProcessEnvelope1(Envelope env)
        {
            _lastIncomingEnvelope1 = env;
        }
    }
}
