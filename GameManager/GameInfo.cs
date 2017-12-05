using System.Collections.Concurrent;
using System.Net;

namespace GameManager
{
    public class GameInfo
    {
        public ConcurrentDictionary<int, IPEndPoint> UserEndPoints { get; set; }
        public short GameId { get; set; }
        public short CurrentPlayerId { get; set; }
        public short[] PlayerPoints { get; set; }
        public short[] PlayerCardAmounts { get; set; }
        public short[] BoardLayout { get; set; }
        public short PlayerWithLR { get; set; }
        public string LastMove { get; set; }

        public GameInfo()
        {
            UserEndPoints = new ConcurrentDictionary<int, IPEndPoint>();
            GameId = 0;
            CurrentPlayerId = 0;
            PlayerPoints = new short[4];
            PlayerCardAmounts = new short[4];
            BoardLayout = new short[84];
            PlayerWithLR = 0;
            LastMove = string.Empty;
        }
    }
}
