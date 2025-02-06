using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DmxLib.Testing
{
    public class AvSink : ISink
    {
        private readonly IPEndPoint _server;
        private readonly byte[] _out = new byte[512];
        private readonly Socket _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public AvSink(string ip, int port)
        {
            _server = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public void Commit()
        {
            while (true)
            {
                _sock.SendTo(_out, _server);
                Thread.Sleep(10);
            }
        }

        public void Update(Universe universe, byte[] values)
        {
            for (var i = 0; i < 512; i++)
            {
                _out[i] = values[i];
            }
        }
    }
}