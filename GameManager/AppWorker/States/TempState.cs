using System;
using ConversationSubsystem;

namespace GameManager.AppWorker.States
{
    public class TempState : AppState
    {
        public TempState(GameManagerAppWorker appWorker)
            : base(appWorker) { }

        public override void Execute()
        {
            int input;
            while ((input = PrintMenu()) != 2)
            {

            }
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
