using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopServer
{
    public class OnlineShopClientHandler
    {
        private readonly TcpClient m_tcpCient;
        private readonly ConcurrentDictionary<string, string> m_currentAccount;
        private readonly ConcurrentDictionary<string, int> m_currentProduct;
        private static ConcurrentDictionary<string, string> CurrentProduct = new ConcurrentDictionary<string, string>(); 
        Random rnd = new Random();

        public OnlineShopClientHandler(TcpClient tcpCient, ConcurrentDictionary<string, string> currentAccount, ConcurrentDictionary<string, int> currentProduct)
        {
            m_tcpCient = tcpCient;
            m_currentAccount = currentAccount;
            m_currentProduct = currentProduct;
            
        }

        //Handle client session
        public void Run(object threadInfo)
        {
            using (m_tcpCient)
            {
                try
                {
                    NetworkStream stream = m_tcpCient.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream);
                    OnlineShop onlineShop = new OnlineShop();

                    m_currentProduct.TryAdd("Cookies", rnd.Next(1, 4));
                    m_currentProduct.TryAdd("Scones", rnd.Next(1, 4));
                    m_currentProduct.TryAdd("Croissants", rnd.Next(1, 4));
                    m_currentProduct.TryAdd("Muffins", rnd.Next(1, 4));
                    m_currentProduct.TryAdd("Cake", rnd.Next(1, 4));

                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        string[] request = line.Split(':');
                        if (3 == request.Length)
                        {
                            string cmd = request[0];
                            if ("CONNECT" == cmd)
                            {
                                onlineShop.Id = request[1];
                                onlineShop.Name = request[2];
                                m_currentAccount.TryAdd(request[1], request[2]);
                                Console.WriteLine($"CONNECT:{request[1]}"); // account number
                                Console.WriteLine($"CONNECTED:{m_currentAccount[request[1]]}"); // name
                                Console.WriteLine($"GET_ORDERS");
                                Console.WriteLine($"ORDERS:");
                            }
                        }
                        if ("PRODUCTS" == line)
                        {
                            Console.WriteLine($"GET_PRODUCTS");
                            string productLine = "PRODUCTS:";
                            foreach (var product in m_currentProduct)
                            {
                                productLine += product.Key + "," + product.Value + "|";
                            }
                            Console.WriteLine($"{productLine.TrimEnd('|')}");
                        }
                        else
                        {
                            if ("DISCONNECT" == line)
                            {
                                Console.WriteLine("DISCONNECT");
                            }
                            else
                            { 
                                OnlineShop online = new OnlineShop();
                                string[] pn = line.Split(':');
                                if (2 == pn.Length) // [0] account Name, [1] product name
                                {
                                    string response = "";
                                    if (m_currentProduct.ContainsKey(pn[1]) && (m_currentProduct[pn[1]] > 0))
                                    {
                                        m_currentProduct[pn[1]]--;
                                        Console.WriteLine($"GET_ORDERS");
                                        CurrentProduct.TryAdd(pn[1], pn[0]);
                                        response = "ORDERS:" + string.Join("|", from product in CurrentProduct select $"{product.Key},1,{product.Value}");
                                        Console.WriteLine(response);
                                        //orderLine += pn[1] + ",1," + pn[0] + "|";
                                        //Console.WriteLine($"{orderLine.TrimEnd('|')}");
                                        Console.WriteLine($"PURCHASE:{pn[1]}");
                                        Console.WriteLine($"DONE");
                                        writer.WriteLine($"GET_ORDERS");
                                        writer.Flush();
                                    }
                                    else if (!m_currentProduct.ContainsKey(pn[1]))
                                    {
                                        Console.WriteLine($"NOT_VALID");
                                        writer.WriteLine($"NOT_VALID");
                                        writer.Flush();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"NOT_AVAILABLE");
                                        writer.WriteLine($"NOT_AVAILABLE");
                                        writer.Flush();
                                    
                                    }
                                }
                            }
                        }
                        
                        
                    }
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Network Error");
                }
                catch (IOException)
                {
                    Console.WriteLine("Network Error");
                }
                catch (OutOfMemoryException)
                {
                    Console.WriteLine("Network Error");
                }
            }
        }
    }
}
