using System.Net;

namespace UserApp
{
    public class UserInfo
    {
        public short PlayerId { get; set; }
        public IPEndPoint GameManagerEP { get; set; }
        public bool IsTurn {
            get
            {
                return (PlayerId == CurrentPlayerId);
            }
        }
        public bool AcceptUserInput { get; set; }

        // Game Info
        public short GameId { get; set; }
        public short CurrentPlayerId { get; set; }
        public short[] PlayerPoints { get; set; }
        public short[] PlayerCardAmounts { get; set; }
        public short[] BoardLayout { get; set; }
        public short PlayerWithLR { get; set; }
        public string LastMove { get; set; }

        public UserInfo()
        {
            PlayerId = -1;
            GameManagerEP = null;
            AcceptUserInput = true;

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
