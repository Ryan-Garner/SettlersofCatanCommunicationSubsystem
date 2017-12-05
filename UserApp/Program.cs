using UserApp.AppWorker;

namespace UserApp
{
    class Program
    {
        static void Main(string[] args)
        {
            UserAppWorker appWorker = new UserAppWorker();

            appWorker.Start();
        }
    }
}
