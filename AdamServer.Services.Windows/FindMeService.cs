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
        UdpClient Server = new UdpClient(18888);
        byte[] ResponseData = Encoding.ASCII.GetBytes("I here");
        ILogger<FindMeService> mLogger;

        public FindMeService(IServiceProvider serviceProvider)
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<FindMeService>>();

            mLogger.LogTrace("Start findme service!");
        }

      
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                    var ClientRequestData = Server.Receive(ref ClientEp);
                    var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                    mLogger.LogInformation("Recived {ClientRequest} from {ClientEp}, sending response", ClientRequest, ClientEp.Address.ToString());
                    Server.Send(ResponseData, ResponseData.Length, ClientEp);
                }
            }, stoppingToken);
        }
    }
}
