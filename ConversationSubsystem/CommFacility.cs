using System;
using SharedObjects;

using log4net;
using System.Net;

namespace ConversationSubsystem
{
    public class CommFacility
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommFacility));

        public ConversationDictionary convDictionary;
        public UdpCommunicator udpCommunicator;
        public ConversationFactory factory;
        public AppWorker myAppWorker;

        private CommFacility() { }

        public CommFacility(AppWorker appWorker, ConversationFactory factory)
        {
            myAppWorker = appWorker;
            this.factory = factory;
            factory.ManagingCommFacility = this;
            convDictionary = new ConversationDictionary();
            udpCommunicator = new UdpCommunicator();
        }

        public void Process(Envelope envelope)
        {
            if (envelope == null)
            {
                throw new Exception();
            }

            Conversation conv = convDictionary.GetConv(envelope.message.convId);

            if (conv != null)
            {
                throw new Exception();
            }
            else
            {
                conv = factory.Create(envelope);
                if(convDictionary.TryAdd(conv.FirstEnvelope.message.convId, conv))
                {
                    conv?.Start();
                }
                else
                {
                    Logger.Error("Unable to add conversation to dictionary");
                }
            }

        }

        /// <summary>
        /// This methods setup up all of the components in a CommFacility.  Call this method
        /// sometime after setting the MinPort, MaxPort, and ConversationFactory
        /// </summary>
        public void Initialize()
        {
            udpCommunicator = new UdpCommunicator()
            {
                MinPort = 10000,
                MaxPort = 20000,
                Timeout = 3000,
                EnvelopeHandler = DelegateToConversation
            };

        }

        /// <summary>
        /// This method starts up all active components in the CommFacility.  Call this method
        /// sometime after calling Initalize.
        /// </summary>
        public void Start()
        {
            Logger.Debug("Entering Start");
            udpCommunicator.Start();
            Logger.Debug("Leaving Start");
        }

        /// <summary>
        /// This method starts up all active components in the CommFacility.  Call this method
        /// sometime after calling Initalize.
        /// </summary>
        public void Start(IPEndPoint localEndPoint)
        {
            Logger.Debug("Entering Start");
            udpCommunicator.Start(localEndPoint);
            Logger.Debug("Leaving Start");
        }

        /// <summary>
        /// This method stops all of the active components of a CommFacility and release the
        /// releases (or at least allows them to be garabage collected.  Once stop is called,
        /// a CommFacility cannot be restarted with setting it up from scratch.
        /// </summary>
        public void Stop()
        {
            Logger.Debug("Entering Stop");

            if (udpCommunicator != null)
            {
                udpCommunicator.Stop();
                udpCommunicator = null;
            }

            Logger.Debug("Leaving Stop");
        }

        public Error Send(Envelope env)
        {
            return udpCommunicator.Send(env);
        }

        public bool RemoveConversation(MessageId convId)
        {
            return convDictionary.Remove(convId);
        }

        public void DelegateToConversation(Envelope envelope)
        {
            if (envelope == null) return;

            Conversation conv = convDictionary.GetConv(envelope.message.convId);
            if (conv != null)
            {
                conv.Enqueue(envelope);
            }
            else
            {
                conv = factory.Create(envelope);
                convDictionary.TryAdd(conv.FirstEnvelope.message.convId, conv);
                conv?.Start();
            }
        }


    }
}
