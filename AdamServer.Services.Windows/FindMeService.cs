using AdamServer.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AdamServer.Services.Windows
{
    public class FindMeService : BackgroundService, IFindMeService
    {
        private readonly ILogger<FindMeService> mLogger;
        private readonly Socket mSocket;

        public FindMeService(IServiceProvider serviceProvider)
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<FindMeService>>();
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint ipEndPoint = new(IPAddress.Any, 18000);
            mSocket.Bind(ipEndPoint);

            IPAddress ip = IPAddress.Parse("224.5.6.7");
            mSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

            mLogger.LogTrace("Start FindMe service!");
        }

      
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    byte[] byteArray = new byte[10];
                    Console.WriteLine("Waiting for data..");

                    IPEndPoint ip = new(IPAddress.Any, 0);
                    EndPoint remoteEndPoint = ip;

                    mSocket.ReceiveFrom(byteArray, byteArray.Length, SocketFlags.None, ref remoteEndPoint);
                    //mSocket.Receive(byteArray);
                    EndPoint remoteEndpoint = mSocket.RemoteEndPoint;
                    string str = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                    Console.WriteLine($"Remote ep {remoteEndPoint}");
                    Console.WriteLine("RX: " + str.Trim());
                }
            }, stoppingToken);
        }

        private void ReciveCallback(IAsyncResult ar)
        {
            //throw new NotImplementedException();
        }
    }
}
