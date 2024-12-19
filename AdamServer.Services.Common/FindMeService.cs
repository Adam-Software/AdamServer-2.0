using AdamServer.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AdamServer.Services.Common
{
    public class FindMeService : BackgroundService, IFindMeService
    {
        private readonly ILogger<FindMeService> mLogger;
        private readonly UdpClient mServer;
        private IPEndPoint mRemoteEndPoint = new(IPAddress.Any, 11000);
        public FindMeService(IServiceProvider serviceProvider)
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<FindMeService>>();
            mServer = new UdpClient(11000);
            mLogger.LogTrace("Start FindMe service!");
        }

      
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    mLogger.LogTrace("FindMe service waiting bradcast...");
                    
                    byte[] byteArray = mServer.Receive(ref mRemoteEndPoint);
                    mServer.Send(byteArray, mRemoteEndPoint);

                    mLogger.LogTrace("Remote ep {remoteEndPoint}", mRemoteEndPoint);
                }
            }, stoppingToken);
        }
    }
}
