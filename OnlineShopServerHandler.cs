using Microsoft.SqlServer.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineShopClient
{
    public class OnlineShopServerHandler : IOnlineShopData
    {
        public string HostName { get; set; } = "localhost";
        public int HostPort { get; set; } = 55059;
        public int AccountNo { get; set; }
        public string UserName { get; set; }
        public string ProductName { get; set; }

        private TcpClient m_tcpClient = null;
        
        private StreamReader reader;
        private StreamWriter writer;
        public IProgress<OnlineShop> Progress { get; set; } = null;
        private readonly IOnlineShopData m_onlineShop;

        public void Connect(int accountNo)
        {
            string userName = string.Empty;

            if (accountNo == 1)
                userName = "Kim";
            else if (accountNo == 2)
                userName = "Lee";
            else if (accountNo == 3)
                userName = "Park";

            ThreadPool.QueueUserWorkItem(threadInfo =>
            {
                (string hostName, int hostPort, IProgress<OnlineShop> progress) = ((string, int, IProgress<OnlineShop>))threadInfo;
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(hostName, hostPort);
                        NetworkStream stream = tcpClient.GetStream();
                        reader = new StreamReader(stream);
                        writer = new StreamWriter(stream);
                        writer.WriteLine($"CONNECT:{accountNo}:{userName}");
                        writer.Flush();
                        string line = reader.ReadLine();
                    }
                }
                catch (InvalidOperationException)
                {

                }
                catch (IOException)
                {

                }
                catch (SocketException)
                {

                }
                catch (OutOfMemoryException)
                {

                }
            }, (HostName, HostPort, Progress));
        }

        public void Disconnect()
        {
            Thread thDisconnect = new Thread(threadInfo =>
            {
                (string hostName, int hostPort, IProgress<OnlineShop> progress) = ((string, int, IProgress<OnlineShop>))threadInfo;
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(hostName, hostPort);
                        NetworkStream stream = tcpClient.GetStream();
                        writer = new StreamWriter(stream);
                        writer.WriteLine($"DISCONNECT");
                        writer.Flush();
                    }
                }
                catch (IOException)
                {

                }
                catch (SocketException)
                {

                }
            });
            thDisconnect.Start((HostName, HostPort, Progress));
        }

        public void GetProducts()
        {
            ThreadPool.QueueUserWorkItem(threadInfo =>
            {
                (string hostName, int hostPort, IProgress<OnlineShop> progress) = ((string, int, IProgress<OnlineShop>))threadInfo;
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(hostName, hostPort);
                        NetworkStream stream = tcpClient.GetStream();
                        StreamReader reader = new StreamReader(stream);
                        StreamWriter writer = new StreamWriter(stream);
                        writer.WriteLine($"PRODUCTS");
                        writer.Flush();
                    }
                }
                catch (IOException)
                {
                    //
                }
                catch (SocketException)
                {
                    //
                }
                catch (OutOfMemoryException)
                {
                    //
                }
            }, (HostName, HostPort, Progress));
        }

        public void GetOrders(string AccountName, string ProductName)
        {
            ThreadPool.QueueUserWorkItem(threadInfo =>
            {
                (string hostName, int hostPort, string accountName, string productName) = ((string, int, string, string))threadInfo;
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(hostName, hostPort);
                        NetworkStream stream = tcpClient.GetStream();
                        StreamReader reader = new StreamReader(stream);
                        StreamWriter writer = new StreamWriter(stream);
                        string line = reader.ReadLine();
                        writer.WriteLine($"GETORDERS,{accountName},{productName}");
                        writer.Flush();
                    }
                }
                catch (IOException)
                {
                    //
                }
                catch (SocketException)
                {
                    //
                }
                catch (OutOfMemoryException)
                {
                    //
                }
            }, (HostName, HostPort, AccountName, ProductName));
        }
        public void Purchase(string AccountName, string ProductName)
        {
            ThreadPool.QueueUserWorkItem(threadInfo =>
            {
                (string hostName, int hostPort, string accountName, string productName) = ((string, int,string, string))threadInfo;
                try
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(hostName, HostPort);
                        NetworkStream stream = tcpClient.GetStream();
                        reader = new StreamReader(stream);
                        writer = new StreamWriter(stream);
                        
                        writer.WriteLine($"{accountName}:{productName}");
                        writer.Flush();
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            if ("GET_ORDERS" == line)
                            {
                                //OnlineShopClientForm.onlineShopClientForm.lstProject.Items.Add(productName + "1" + accountName);
                            }
                            if ("NOT_VALID" == line)
                            {
                                MessageBox.Show("The specified product is unavailable", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if ("NOT_AVAILABLE" == line)
                            {
                                MessageBox.Show("The product is unavailable", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                
                            }
                        }
                    }

                }
                catch (IOException)
                {
                    //
                }
                catch (SocketException)
                {
                    //
                }
                catch (OutOfMemoryException)
                {
                    //
                }
            }, (HostName, HostPort, AccountName, ProductName));
        }
    }
}
