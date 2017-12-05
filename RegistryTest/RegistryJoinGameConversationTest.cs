using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Registry;
using System.Net;
using ConversationSubsystem;
using Registry.Conversations;
using Messages;
using System.Threading;
using Registry.AppWorker;

namespace RegistryTest
{
    [TestClass]
    public class RegistryJoinGameConversationTest
    {
        public class TestAppWorker : RegistryAppWorker
        {
            public IPEndPoint registryEndpoint;

            public TestAppWorker(RegistryData registryData, IPEndPoint registryEndpoint)
            {
                RegistryData = registryData;
                this.registryEndpoint = registryEndpoint;
                commFacility = new CommFacility(this, new RegistryConversationFactory());
            }

            public void StartTest()
            {
                commFacility.Initialize();
                commFacility.Start(registryEndpoint);
            }

            public void StopTest()
            {
                commFacility.Stop();
            }
        }

        private Envelope _lastIncomingEnvelope1;

        [TestMethod]
        public void Registry_JoinGameConversation_Test()
        {
            IPEndPoint registryEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);

            RegistryData registryData = new RegistryData();
            TestAppWorker testAppWorker = new TestAppWorker(registryData, registryEndPoint);
            testAppWorker.StartTest();

            var comm1 = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };
            comm1.Start();

            Message msg1 = new RequestGameMessage();
            Envelope env = new Envelope(msg1, registryEndPoint);

            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));
            Assert.IsNull(registryData.GetAvailableGameManager());

            comm1.Send(env);

            Thread.Sleep(1000);

            /*
            //The conversation happens to fast. After the sleep, the conv is already gone
            Assert.IsNotNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));
            */

            Assert.AreNotSame(msg1, _lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);
            Assert.AreEqual(msg1.convId, _lastIncomingEnvelope1.message.convId);
            GameInfoMessage msg2 = _lastIncomingEnvelope1.message as GameInfoMessage;
            Assert.IsNotNull(msg2);

            Assert.IsNotNull(registryData.GetAvailableGameManager());
            Assert.IsNull(testAppWorker.commFacility.convDictionary.GetConv(msg1.convId));

            //-------------STOPPING------------//
            testAppWorker.StopTest();
            comm1.Stop();
        }

        private void ProcessEnvelope1(Envelope env)
        {
            _lastIncomingEnvelope1 = env;
        }
    }
}
