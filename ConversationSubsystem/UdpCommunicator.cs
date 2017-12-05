using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using System;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

using Messages;
using SharedObjects;
using log4net;

namespace ConversationSubsystem
{
    public delegate void IncomingEnvelopeHandler(Envelope env);

    public class UdpCommunicator
    {
        #region Private Data Members
        public UdpClient _myUdpClient { get; private set; }
        private bool _keepGoing;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(UdpCommunicator));
        private static readonly ILog LoggerDeep = LogManager.GetLogger(typeof(UdpCommunicator) + "_Deep");
        private static readonly object StartStopLock = new object();
        #endregion

        #region Public Properties
        public int MinPort { get; set; }
        public int MaxPort { get; set; }
        public int Timeout { get; set; }
        public int Port => ((IPEndPoint)_myUdpClient?.Client.LocalEndPoint)?.Port ?? 0;
        public IncomingEnvelopeHandler EnvelopeHandler { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method starts up the UdpCommunicator by a) creating a UdpClient that is bound to a free port between the specified MinPort and MaxPort and b)
        /// starting a thread that looks for incoming messsages.  When that thread gets an incoming message, it will package it up in an envelope and callback
        /// to the specified EnvelopeHandler, which should be a method in the CommFacility
        /// </summary>
        public void Start()
        {
            Logger.Info("Start communicator");

            ValidPorts();

            lock (StartStopLock)
            {
                if (_keepGoing) Stop();

                var portToTry = FindAvailablePort(MinPort, MaxPort);
                if (portToTry > 0)
                {
                    try
                    {
                        var localEp = new IPEndPoint(IPAddress.Any, portToTry);
                        _myUdpClient = new UdpClient(localEp);
                        _keepGoing = true;
                        Task.Factory.StartNew(Receive);
                    }
                    catch (SocketException)
                    {
                    }
                }

                if (!_keepGoing)
                    throw new ApplicationException($"Cannot bind the socket to a port {portToTry}");
            }
        }

        /// <summary>
        /// This attempts to create a udp client with the localEndPoint passed in
        /// </summary>
        /// <param name="localEndPoint">localEndPoint to attempt to connect to</param>
        public void Start(IPEndPoint localEndPoint)
        {
            Logger.Info("Start communicator");

            ValidPorts();

            lock (StartStopLock)
            {
                if (_keepGoing) Stop();

                _keepGoing = false;

                try
                {
                    _myUdpClient = new UdpClient(localEndPoint);
                    _keepGoing = true;
                    Task.Factory.StartNew(Receive);
                }
                catch (SocketException)
                {
                }
                

                if (!_keepGoing)
                    throw new ApplicationException($"Cannot bind the socket to endpoint: {localEndPoint.ToString()}");
            }
        }

        /// <summary>
        /// This method stops the UdpCommunicator by a) closing its UdpClient and b) setting stopping the receive thread.
        /// </summary>
        public void Stop()
        {
            Logger.Debug("Entering Stop");

            lock (StartStopLock)
            {
                _keepGoing = false;

                if (_myUdpClient != null)
                {
                    _myUdpClient.Close();
                    _myUdpClient = null;
                }
            }

            Logger.Info("Communicator Stopped");
        }

        /// <summary>
        /// This method sends the message in an envelope to the EndPoint of the envelope.  Note that just the message is
        /// send out the UdpClient, not the whole envelope.
        /// </summary>
        /// <param name="outgoingEnvelope"></param>
        /// <returns></returns>
        public Error Send(Envelope outgoingEnvelope)
        {
            if (outgoingEnvelope == null)
            {
                Logger.Warn("No envelope to send");
                return new Error() { Text = "Send failed: no envelope provided" };
            }
            if (!outgoingEnvelope.IsValidToSend)
            {
                Logger.Warn("Invalid Envelope");
                return new Error() { Text = "Send failed: Invalid envelope" };
            }

            Error error = null;

            byte[] bytesToSend = outgoingEnvelope.message.Encode();
            Logger.DebugFormat($"Send out: {Encoding.ASCII.GetString(bytesToSend)} to {outgoingEnvelope.remoteEndPoint}");

            try
            {
                _myUdpClient.Send(bytesToSend, bytesToSend.Length, outgoingEnvelope.remoteEndPoint);
            }
            catch (Exception err)
            {
                error = new Error()
                {
                    Text = $"Cannnot send a {outgoingEnvelope.message.GetType().Name} to {outgoingEnvelope.remoteEndPoint}: {err.Message}"
                };
                Logger.Warn(error.Text);
            }
            return error;
        }

        public Error JoinMulticastGroup(IPAddress groupAddress)
        {
            Error error = null;
            try
            {
                _myUdpClient.JoinMulticastGroup(groupAddress);
            }
            catch (Exception err)
            {
                error = new Error() { Text = $"Cannot join multicast group: {err}" };
            }
            return error;
        }

        public Error DropMulticastGroup(IPAddress groupAddress)
        {
            Error error = null;
            try
            {
                _myUdpClient.DropMulticastGroup(groupAddress);
            }
            catch (Exception err)
            {
                error = new Error() { Text = $"Cannot join multicast group: {err}" };
            }
            return error;
        }
        #endregion

        #region Private Methods
        private void Receive()
        {
            while (_keepGoing)
            {
                var env = ReceiveOne();
                if (env != null)
                    EnvelopeHandler?.Invoke(env);
            }
        }

        private Envelope ReceiveOne()
        {
            Envelope result = null;

            IPEndPoint senderEndPoint;
            byte[] receivedBytes = ReceiveBytes(Timeout, out senderEndPoint);
            if (receivedBytes != null && receivedBytes.Length > 0)
            {
                var message = Message.DecodeMessage(receivedBytes);
                if (message != null)
                {
                    result = new Envelope(message, senderEndPoint);
                    Logger.Debug(
                        $"Just received message, Nr={result.message.msgId?.ToString() ?? "null"}, " +
                        $"Conv={result.message.convId?.ToString() ?? "null"}, " +
                        $"Type={result.message.GetType().Name}, " +
                        $"From={result.remoteEndPoint?.ToString() ?? "null"}");
                }
                else
                {
                    Logger.Error($"Cannot decode message received from {senderEndPoint}");
                    Logger.Error($"Message={Encoding.ASCII.GetString(receivedBytes)}");
                }
            }

            return result;
        }

        private byte[] ReceiveBytes(int timeout, out IPEndPoint ep)
        {
            ep = null;
            if (_myUdpClient == null) return null;

            byte[] receivedBytes = null;
            _myUdpClient.Client.ReceiveTimeout = timeout;
            ep = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                LoggerDeep.Debug("Try receive bytes from anywhere");
                try
                {
                    receivedBytes = _myUdpClient.Receive(ref ep);
                }
                catch (SocketException err)
                {
                    if (err.SocketErrorCode != SocketError.TimedOut)
                        throw;
                }
                LoggerDeep.Debug("Back from receive");

                if (Logger.IsDebugEnabled)
                {
                    var tmp = Encoding.ASCII.GetString(receivedBytes);
                    Logger.Debug($"Incoming message={tmp}");
                }
            }
            catch (SocketException err)
            {
                if (err.SocketErrorCode != SocketError.TimedOut && err.SocketErrorCode != SocketError.Interrupted)
                {

                }
                    Logger.Warn(err.Message);
            }
            catch (Exception err)
            {
                Logger.Warn(err.Message);
            }
            return receivedBytes;
        }

        private void ValidPorts()
        {
            if ((MinPort != 0 && (MinPort < IPEndPoint.MinPort || MinPort > IPEndPoint.MaxPort)) ||
                (MaxPort != 0 && (MaxPort < IPEndPoint.MinPort || MaxPort > IPEndPoint.MaxPort)))
                throw new ApplicationException("Invalid port specifications");
        }

        private static int FindAvailablePort(int minPort, int maxPort)
        {
            int availablePort = -1;

            Logger.DebugFormat("Find a free port between {0} and {1}", minPort, maxPort);
            for (int possiblePort = minPort; possiblePort <= maxPort; possiblePort++)
            {
                if (!IsUsed(possiblePort))
                {
                    availablePort = possiblePort;
                    break;
                }
            }
            Logger.DebugFormat("Available Port = {0}", availablePort);
            return availablePort;
        }

        private static bool IsUsed(int port)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var endPoints = properties.GetActiveUdpListeners();
            return endPoints.Any(ep => ep.Port == port);
        }
        #endregion

    }
}
