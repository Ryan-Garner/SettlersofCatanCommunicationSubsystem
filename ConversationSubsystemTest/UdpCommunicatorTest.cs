using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ConversationSubsystem;
using Messages;
using SharedObjects;
using System;

namespace ConversationSubsystemTest
{
    [TestClass]
    public class UdpCommunicatorTest
    {
        private Envelope _lastIncomingEnvelope1;
        private Envelope _lastIncomingEnvelope2;
        private Envelope _lastIncomingEnvelope3;
        private Envelope _lastIncomingEnvelope4;
        private Envelope _lastIncomingEnvelope5;

        [TestMethod]
        public void UdpCommunicator_SimpleSendReceives()
        {
            var comm1 = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope1
            };
            comm1.Start();

            var comm2 = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope2
            };
            comm2.Start();

            Random rand = new Random();
            short gameId = (short)rand.Next(1, 1000);

            StartGameMessage msg = new StartGameMessage(gameId);
            IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Loopback, comm2.Port);
            Envelope env = new Envelope(msg, targetEndPoint);

            comm1.Send(env);

            Thread.Sleep(100);

            Assert.AreNotSame(msg, _lastIncomingEnvelope2);
            Assert.IsNotNull(_lastIncomingEnvelope2);
            Assert.IsNotNull(_lastIncomingEnvelope2.message);
            Assert.AreEqual(msg.msgId, _lastIncomingEnvelope2.message.msgId);
            Assert.AreEqual(msg.convId, _lastIncomingEnvelope2.message.convId);
            StartGameMessage msg2 = _lastIncomingEnvelope2.message as StartGameMessage;
            Assert.IsNotNull(msg2);
            Assert.AreEqual(msg.GameId, msg2.GameId);

            AckMessage msg3 = new AckMessage(msg2.convId, msg2.GameId);
            Assert.AreNotEqual(msg2.msgId, msg3.msgId);
            Assert.AreEqual(msg2.convId, msg3.convId);
            targetEndPoint = new IPEndPoint(IPAddress.Loopback, comm1.Port);
            Envelope env3 = new Envelope(msg3, targetEndPoint);
            comm2.Send(env3);

            Thread.Sleep(100);

            Assert.AreNotSame(msg3, _lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1);
            Assert.IsNotNull(_lastIncomingEnvelope1.message);
            Assert.AreEqual(msg3.msgId, _lastIncomingEnvelope1.message.msgId);
            Assert.AreEqual(msg3.convId, _lastIncomingEnvelope1.message.convId);
            AckMessage msg4 = _lastIncomingEnvelope1.message as AckMessage;
            Assert.IsNotNull(msg4);
            Assert.AreEqual(msg3.GameId, msg4.GameId);

            comm1.Stop();
            comm2.Stop();
        }
        
        private void ProcessEnvelope1(Envelope env)
        {
            _lastIncomingEnvelope1 = env;
        }

        private void ProcessEnvelope2(Envelope env)
        {
            _lastIncomingEnvelope2 = env;
        }
        
        [TestMethod]
        public void UdpCommunicator_SendingOfBadEnvelopes()
        {
            var commSender = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = NoOp
            };
            commSender.Start();

            var commReciever = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope3
            };
            commReciever.Start();

            Random rand = new Random();
            short gameId = (short)rand.Next(1, 1000);

            // Send a message to a non-existant remote end point
            //     Expected behavior - no error, but no delivery
            var msg = new StartGameMessage(gameId);
            var targetEndPoint = new IPEndPoint(IPAddress.Loopback, 1012);
            var env = new Envelope(msg, targetEndPoint);
            var error = commSender.Send(env);
            Assert.IsNull(error);
            Assert.IsNull(_lastIncomingEnvelope3);

            // Send a message to a remote end point with a 0 for the port
            //     Expected behavior - error
            env.remoteEndPoint = new IPEndPoint(IPAddress.Loopback, 0);
            error = commSender.Send(env);
            Assert.IsNotNull(error);
            Assert.IsNull(_lastIncomingEnvelope3);

            // Send a message to a remote end point with a 0.0.0.0 for the address
            //     Expected behavior - error
            env.remoteEndPoint = new IPEndPoint(IPAddress.Any, 1245);
            error = commSender.Send(env);
            Assert.IsNotNull(error);
            Assert.IsNull(_lastIncomingEnvelope3);

            // Send to a null remote end point            
            //      Expected behavior -- error
            env.remoteEndPoint = null;
            error = commSender.Send(env);
            Assert.IsNotNull(error);
            Assert.IsNull(_lastIncomingEnvelope3);

            // Send a null message            
            //      Expected behavior -- error
            env.message = null;
            env.remoteEndPoint = new IPEndPoint(IPAddress.Loopback, 1012);
            error = commSender.Send(env);
            Assert.IsNotNull(error);
            Assert.IsNull(_lastIncomingEnvelope3);

            // Send a null envelope            
            //      Expected behavior -- error
            error = commSender.Send(null);
            Assert.IsNotNull(error);
            Assert.IsNull(_lastIncomingEnvelope3);

            commSender.Stop();
            commReciever.Stop();
        }
        private void NoOp(Envelope env)
        {
            _lastIncomingEnvelope2 = env;
        }

        private void ProcessEnvelope3(Envelope env)
        {
            _lastIncomingEnvelope3 = env;
        }
        
        [TestMethod]
        public void UdpCommunicator_ReceivingOfBadMessages()
        {
            // Setup a socket from which to send bad data 
            var localEp = new IPEndPoint(IPAddress.Any, 0);
            var sender = new UdpClient(localEp);

            var commReciever = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope4
            };
            commReciever.Start();

            Random rand = new Random();
            short gameId = (short)rand.Next(1, 1000);

            byte[] bytesToSend = Encoding.ASCII.GetBytes("Garabage");
            var targetIpAddress = new IPEndPoint(IPAddress.Loopback, commReciever.Port);
            sender.Send(bytesToSend, bytesToSend.Length, targetIpAddress);

            // Give the receiver time to run
            Thread.Sleep(100);

            // No message should received
            Assert.IsNull(_lastIncomingEnvelope4);

            sender.Close();
            commReciever.Stop();
        }

        private void ProcessEnvelope4(Envelope env)
        {
            _lastIncomingEnvelope4 = env;
        }
        
        [TestMethod]
        public void UdpCommunicator_Multicast()
        {
            var commSender = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 10999,
                Timeout = 1000,
                EnvelopeHandler = NoOp
            };
            commSender.Start();

            var commReciever = new UdpCommunicator()
            {
                MinPort = 5000,
                MaxPort = 5000,
                Timeout = 1000,
                EnvelopeHandler = ProcessEnvelope5
            };
            commReciever.Start();

            Random rand = new Random();
            short gameId = (short)rand.Next(1, 1000);

            // Note, we can't test multiple receivers on the same host (localhost), because the
            // port number has to be the same for all receivers

            // Have the receivers join a Group Multicast address
            var multiCastAddress = new IPAddress(new byte[] { 224, 1, 1, 2 });
            commReciever.JoinMulticastGroup(multiCastAddress);

            // Send message to Group Multicast address
            var msg = new StartGameMessage(gameId);
            var targetEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);
            var env = new Envelope(msg, targetEndPoint);

            commSender.Send(env);

            Thread.Sleep(100);

            Assert.AreNotSame(msg, _lastIncomingEnvelope5);
            Assert.IsNotNull(_lastIncomingEnvelope5);
            Assert.IsNotNull(_lastIncomingEnvelope5.message);
            Assert.AreEqual(msg.msgId, _lastIncomingEnvelope5.message.msgId);
            Assert.AreEqual(msg.convId, _lastIncomingEnvelope5.message.convId);
            StartGameMessage msg2 = _lastIncomingEnvelope5.message as StartGameMessage;
            Assert.IsNotNull(msg2);
            Assert.AreEqual(msg.GameId, msg2.GameId);

            commReciever.DropMulticastGroup(multiCastAddress);
            commReciever.Stop();
            commSender.Stop();
        }

        private void ProcessEnvelope5(Envelope env)
        {
            _lastIncomingEnvelope5 = env;
        }

    }
}
