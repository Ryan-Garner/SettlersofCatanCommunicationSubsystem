using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharedObjects
{
    public static class EndPointParser
    {
        public static IPEndPoint ParseEndPoint(string hostAndPort)
        {
            var host = "";
            var port = 0;
            var defaultEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (string.IsNullOrWhiteSpace(hostAndPort)) return defaultEndPoint;

            var tmp = hostAndPort.Split(':');
            if (tmp.Length == 1)
            {
                host = hostAndPort;
            }
            else if (tmp.Length >= 2)
            {
                host = tmp[0].Trim();
                int.TryParse(tmp[1].Trim(), out port);
            }

            if (string.IsNullOrWhiteSpace(host)) return defaultEndPoint;

            var address = LookupAddress(host);
            defaultEndPoint = new IPEndPoint(address, port);

            return defaultEndPoint;
        }


        public static IPAddress LookupAddress(string host)
        {
            if (string.IsNullOrWhiteSpace(host)) return IPAddress.Any;

            IPAddress result = null;
            var addressList = Dns.GetHostAddresses(host);
            for (var i = 0; i < addressList.Length && result == null; i++)
                if (addressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    result = addressList[i];
            return result;
        }
    }
}
