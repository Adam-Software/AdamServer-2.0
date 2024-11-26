using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdamServer.Services.Common
{
    public class WebApiHandlerService : IWebApiHandlerService
    {
        #region Services

        private readonly ILogger<WebApiHandlerService> mLogger;
        private readonly IShellCommandService mShellCommandService;
        
        #endregion

        public WebApiHandlerService(IServiceProvider serviceProvider) 
        { 
            mLogger = serviceProvider.GetService<ILogger<WebApiHandlerService>>();
            mShellCommandService = serviceProvider.GetService<IShellCommandService>();
        }

        public async Task<string> ExecuteCommandAsync(string command)
        {
            var result = await mShellCommandService.ExecuteCommandAsync(command);
            mLogger.LogTrace("Result execute command {command} by service", command);
            mLogger.LogTrace("{result}", result);
            return result;
        }
    }
}
