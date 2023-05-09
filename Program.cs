using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Thread thServer = new Thread(new OnlineShopServer(cancellationTokenSource.Token).Start);
            thServer.Start();
            Console.WriteLine("Press Q to shut down");
            while (thServer.IsAlive && (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Q))
                ;
            cancellationTokenSource.Cancel();
            Console.WriteLine("Server is shutting down");
            Console.ReadKey();
        }
    }
}
