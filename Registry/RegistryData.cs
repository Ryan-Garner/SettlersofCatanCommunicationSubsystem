using System.Collections.Concurrent;
using System.Net;

namespace Registry
{
    public class RegistryData
    {
        public class GameInfo
        {
            public short Players { get; set; }
            public bool GameActive { get; set; }
            public short GameId { get; set; }
            public IPEndPoint RemoteEndPoint { get; set; }
        }

        public ConcurrentDictionary<IPEndPoint, GameInfo> ActiveGameManagers { get; private set; }
        private static short _nextGameId;                        // Initialize to 0, which means it will start with game id 1
        private static readonly object MyLock = new object();

        /// <summary>
        /// Constructor, creates new dictionary
        /// </summary>
        public RegistryData()
        {
            ActiveGameManagers = new ConcurrentDictionary<IPEndPoint, GameInfo>();
        }

        public GameInfo GetAvailableGameManager()
        {
            foreach(GameInfo gi in ActiveGameManagers.Values)
            {
                if (!gi.GameActive && gi.Players < 4)
                {
                    return gi;
                }
            }

            return null;
        }

        public static short GetNextGameId()
        {
            lock (MyLock)
            {
                if (_nextGameId == short.MaxValue)
                    _nextGameId = 0;
                ++_nextGameId;
            }
            return _nextGameId;
        }

        public bool AddGame(GameInfo gameInfo)
        {
            return ActiveGameManagers.TryAdd(gameInfo.RemoteEndPoint, gameInfo);
        }

        public GameInfo GetGame(IPEndPoint endPoint)
        {
            GameInfo value = null;
            ActiveGameManagers.TryGetValue(endPoint, out value);

            return value;
        }
    }
}
