using AdamServer.Interfaces.WebApiHandlerService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AdamServer.Interfaces.WebApiHandlerService
{
    public interface IPythonHandler
    {
        public Task<string> ExecutePythonCommandAsync(PythonCommand command);
        public Task StopExecutePythonCommandAsync();
    }

    public class PythonHandler : IPythonHandler
    {
        #region Services

        private readonly ILogger<PythonHandler> mLogger;
        private readonly IShellCommandService mShellCommandService;

        #endregion

        public PythonHandler(IServiceProvider serviceProvider) 
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<PythonHandler>>();
            mShellCommandService = serviceProvider.GetRequiredService<IShellCommandService>();
        }
        public async Task<string> ExecutePythonCommandAsync(PythonCommand command)
        {
            var result = await mShellCommandService.ExecuteCommandAsync(command.TextCommand);
            mLogger.LogTrace("Result execute command {command} by service", command);
            mLogger.LogTrace("{result}", result);
            return result;
        }

        public Task StopExecutePythonCommandAsync()
        {
            return Task.CompletedTask; 
        }
    }
}
