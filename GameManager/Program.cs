using GameManager.AppWorker;

namespace GameManager
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManagerAppWorker appWorker = new GameManagerAppWorker(args);

            appWorker.Start();
        }
    }
}
