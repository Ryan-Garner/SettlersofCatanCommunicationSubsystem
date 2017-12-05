using Registry.AppWorker;

namespace Registry
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryAppWorker appWorker = new RegistryAppWorker();

            appWorker.Start();
        }
    }
}
