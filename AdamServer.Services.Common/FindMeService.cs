using AdamServer.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace AdamServer.Services.Common
{
    public class FindMeService : BackgroundService, IFindMeService
    {
        private readonly ILogger<FindMeService> mLogger;
        private readonly UdpClient mServer;
        private readonly byte[] mReply = Encoding.UTF8.GetBytes("pong");

        public FindMeService(IServiceProvider serviceProvider)
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<FindMeService>>();
            mServer = new UdpClient(11000);
            
            mLogger.LogTrace("Start FindMe service!");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            mLogger.LogTrace("FindMe service waiting bradcast at... ");
            
            try
            {
                while (!stoppingToken.IsCancellationRequested) 
                {
                    UdpReceiveResult reciveResult = await mServer.ReceiveAsync(stoppingToken);

                    string reciveMessage = Encoding.UTF8.GetString(reciveResult.Buffer);
                    mLogger.LogTrace("Recive message {reciveMessage} from remote ep {remoteEndPoint}", reciveMessage, reciveResult.RemoteEndPoint);

                    if(reciveMessage == "ping")
                    {
                        await mServer.SendAsync(mReply, reciveResult.RemoteEndPoint, stoppingToken);
                        mLogger.LogTrace("Send {reply} to remote ep {remoteEndPoint}", Encoding.UTF8.GetString(mReply), reciveResult.RemoteEndPoint);
                    }
                }
            }
            catch (OperationCanceledException) 
            { 
                mLogger.LogTrace("Udp server service was canceled"); 
            }
            catch (Exception ex) 
            { 
                mLogger.LogError("Error {Exception}", ex.Message); 
            }
            finally 
            { 
                mServer.Close(); 
            }
        }

        public override void Dispose()
        {
            mServer?.Dispose();
        }
    }
}
