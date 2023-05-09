using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopServer
{
    public class OnlineShopServer
    {
        public readonly CancellationToken m_cancellationToken;
        public IPAddress ServerIp { get; set; } = IPAddress.Any;
        public int ServerPort { get; set; } = 55059;
        private readonly ConcurrentDictionary<string, string> m_currentAccount = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, int> m_currentProduct = new ConcurrentDictionary<string, int>();
        public OnlineShopServer(CancellationToken cancellationToken) => m_cancellationToken = cancellationToken;

        // Handle new client connections
        public void Start()
        {
            try
            {
                TcpListener listener = new TcpListener(ServerIp, ServerPort);
                listener.Start();
                m_cancellationToken.Register(listener.Stop);
                while (!m_cancellationToken.IsCancellationRequested)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    OnlineShopClientHandler handler = new OnlineShopClientHandler(tcpClient, m_currentAccount, m_currentProduct);
                    ThreadPool.QueueUserWorkItem(handler.Run);
                }
            }
            catch (SocketException)
            {
                //
            }
        }
        public void StartServer() => new Thread(Start).Start();
    }
}
