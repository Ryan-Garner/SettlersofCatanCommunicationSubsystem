using ConversationSubsystem;
using Messages;
using SharedObjects;
using System;
using System.Net;

namespace GameManager.AppWorker.States
{
    public class StartUpState : AppState
    {
        private string[] args;
        public StartUpState(GameManagerAppWorker appWorker, string[] args)
            : base(appWorker)
        {
            this.args = args;
        }

        public override void Execute()
        {
            Envelope envelope = null;
            if (args.Length > 1)
            {
                try
                {
                    short pid = Convert.ToInt16(args[0]);
                    short seq = Convert.ToInt16(args[1]);
                    MessageId msgId = new MessageId() { Pid = pid, Seq = seq };
                    Message m = new AckMessage(msgId, -1);
                    envelope = new Envelope(m, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            myAppWorker.commFacility.Process(envelope);

            // TODO: Change AppState
            myAppWorker.SetState(((GameManagerAppWorker)myAppWorker).TempState);
        }

        static int PrintMenu()
        {
            int input = -1;

            Console.WriteLine("1: Request Game");
            Console.WriteLine("2: Exit");
            Console.WriteLine();
            Console.Write("Input: ");

            try
            {
                input = Convert.ToInt32(Console.ReadLine());
            }
            catch { }

            Console.WriteLine();

            return input;
        }
    }
}
