using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AdamServer.Services.Common
{
    public class StreamServerService : IStreamServerService
    {
        private readonly ILogger<StreamServerService> mLoggerService;
        public StreamServerService(IServiceProvider serviceProvider) 
        {
            mLoggerService = serviceProvider.GetRequiredService<ILogger<StreamServerService>>();
        }


        public async Task StartListening()
        {
            var listener = new TcpListener(IPAddress.Loopback, 18000);
            listener.Start();
            var server = await listener.AcceptTcpClientAsync();

            await using var networkStream = server.GetStream();

            while (true)
            {
                // again, assume that maximum message length is 255 bytes
                var msgLengthBuffer = new byte[1];
                await networkStream.ReadExactlyAsync(msgLengthBuffer);

                var msgPayloadBuffer = new byte[msgLengthBuffer[0]];
                await networkStream.ReadExactlyAsync(msgPayloadBuffer);

                mLoggerService.LogTrace($"Client: received message '{Encoding.UTF8.GetString(msgPayloadBuffer)}'");
            }

        }
    }
}
