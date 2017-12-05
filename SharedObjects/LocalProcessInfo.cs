using System;

namespace SharedObjects
{
    public class LocalProcessInfo
    {
        private static LocalProcessInfo _instance;
        private static readonly object MyLock = new object();

        public static LocalProcessInfo Instance 
        {
            get
            {
                lock (MyLock)
                {
                    if (_instance==null)
                        _instance = new LocalProcessInfo();
                }
                return _instance;                
            }
        }

        public short ProcessId { get; set; }

        public DateTime StartTime { get; set; }
    }
}
