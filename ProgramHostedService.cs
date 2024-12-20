﻿using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdamServer
{
    [Obsolete]
    public sealed class ProgramHostedService : IHostedService, IHostedLifecycleService
    {
        #region Services
        
        private readonly ILogger<ProgramHostedService> mLoggerService;
        private readonly IFindMeService mFindMeService;

        #endregion

        #region Var

        private readonly Task mCompletedTask = Task.CompletedTask;

        #endregion

        public ProgramHostedService(IServiceProvider serviceProvider)
        {
            mLoggerService = serviceProvider.GetService<ILogger<ProgramHostedService>>();
            mFindMeService = serviceProvider.GetService<IFindMeService>();  

            var option = serviceProvider.GetService<IAppSettingsOptionsService>();
            var appLifetime = serviceProvider.GetService<IHostApplicationLifetime>();

            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogTrace("1. StartingAsync has been called.");
            return mCompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogTrace("2. StartAsync has been called.");
            return mCompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogTrace("3. StartedAsync has been called.");
            //mFindMeService.ExecuteAsync(cancellationToken);
            return mCompletedTask;
        }

        private void OnStarted()
        {
            
            mLoggerService.LogTrace("4. OnStarted has been called.");
        }

        private void OnStopping()
        {
            mLoggerService.LogTrace("5. OnStopping has been called.");
        }


        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogTrace("6. StoppingAsync has been called.");
            return mCompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogTrace("7. StopAsync has been called.");
            //mFindMeService.StopAsync(cancellationToken);

            return mCompletedTask;
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            mLoggerService.LogTrace("8. StoppedAsync has been called.");
            return mCompletedTask;
        }

        private void OnStopped()
        {
            mLoggerService.LogTrace("9. OnStopped has been called.");
        }
    }
}
