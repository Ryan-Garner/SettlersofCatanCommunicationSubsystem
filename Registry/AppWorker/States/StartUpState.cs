using ConversationSubsystem;
using System;

namespace Registry.AppWorker.States
{
    public class StartUpState : AppState
    {
        public StartUpState(RegistryAppWorker appWorker)
            : base(appWorker) { }

        public override void Execute()
        {
            int input;

            while ((input = PrintMenu()) != 2)
            {
                if (input == 1)
                {
                    var games = ((RegistryAppWorker)myAppWorker).RegistryData.ActiveGameManagers.Values;
                    if (games.Count == 0)
                    {
                        Console.WriteLine("No games active");
                    }
                    else
                    {
                        Console.WriteLine("GameId of current games: ");
                        foreach (RegistryData.GameInfo game in ((RegistryAppWorker)myAppWorker).RegistryData.ActiveGameManagers.Values)
                        {
                            Console.WriteLine(game.GameId);
                        }
                    }
                }
                else if (input == 2)
                {
                    Console.WriteLine("Bye");
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }

            myAppWorker.Stop();
        }

        private int PrintMenu()
        {
            int input = -1;
            Console.WriteLine("1: View Games");
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
