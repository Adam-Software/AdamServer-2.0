using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AdamServer
{
    public class ProgramHostedService : IHostedService, IDisposable
    {
        public ProgramHostedService(IServiceProvider serviceProvider)
        {
            var option = serviceProvider.GetService<IAppSettingsOptions>();
            Console.WriteLine(option.Test3);
        }

        public void Dispose()
        {
            Debug.WriteLine("App Dispose");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("App start");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("App stop");
            return Task.CompletedTask;
        }
    }
}
