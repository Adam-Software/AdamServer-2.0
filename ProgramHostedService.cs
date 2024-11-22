using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdamServer
{
    public sealed class ProgramHostedService : IHostedService, IHostedLifecycleService
    {
        private readonly Task mCompletedTask = Task.CompletedTask;
        private readonly ILogger<ProgramHostedService> mLoggerService;

        public ProgramHostedService(IServiceProvider serviceProvider)
        {
            mLoggerService = serviceProvider.GetService<ILogger<ProgramHostedService>>();

            var option = serviceProvider.GetService<IAppSettingsOptionsService>();
            var appLifetime = serviceProvider.GetService<IHostApplicationLifetime>();

            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            mLoggerService.LogInformation("option.Test3 = {option.Test3}", option.Test);
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogInformation("1. StartingAsync has been called.");
            return mCompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogInformation("2. StartAsync has been called.");
            return mCompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogInformation("3. StartedAsync has been called.");
            return mCompletedTask;
        }

        private void OnStarted()
        {
            mLoggerService.LogInformation("4. OnStarted has been called.");
        }

        private void OnStopping()
        {
            mLoggerService.LogInformation("5. OnStopping has been called.");
        }


        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogInformation("6. StoppingAsync has been called.");
            return mCompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogInformation("7. StopAsync has been called.");
            return mCompletedTask;
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogInformation("8. StoppedAsync has been called.");
            return mCompletedTask;
        }

        private void OnStopped()
        {
            mLoggerService.LogInformation("9. OnStopped has been called.");
        }

    }
}
