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
        byte[] mReply = Encoding.UTF8.GetBytes("pong");

        public FindMeService(IServiceProvider serviceProvider)
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<FindMeService>>();

            mServer = new UdpClient(11000)
            {
                 //EnableBroadcast = true
            };

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
                    
                    var reciveMessage = Encoding.UTF8.GetString(byteArray);
                    mLogger.LogTrace("Recive message {reciveMessage} from remote ep {remoteEndPoint}", reciveMessage, mRemoteEndPoint);

                    mServer.Send(mReply, mRemoteEndPoint);
                    mLogger.LogTrace("Send {reply} to remote ep {remoteEndPoint}", mReply, mRemoteEndPoint);
                }
            }, stoppingToken);
        }
    }
}
