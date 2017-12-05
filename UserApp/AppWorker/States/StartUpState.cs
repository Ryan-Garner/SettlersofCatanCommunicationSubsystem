using ConversationSubsystem;
using Messages;
using System;
using System.Net;

namespace UserApp.AppWorker.States
{
    public class StartUpState : AppState
    {
        public StartUpState(UserAppWorker appWorker)
            : base(appWorker) { }

        public override void Execute()
        {
            int input;
            
            while ((input = PrintMenu()) != 2)
            {
                if (input == 1)
                {
                    Console.WriteLine("Requesting Game...");
                    Console.WriteLine();

                    Message m = new RequestGameMessage();
                    Envelope e = new Envelope(m, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));

                    try
                    {
                        myAppWorker.commFacility.Process(e);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                else if (input == 2)
                {
                    Console.WriteLine("Bye");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    Console.WriteLine();
                }
            }

            // TODO: Change AppState
            myAppWorker.Stop();
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
