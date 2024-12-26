using AdamServer.Interfaces.WebApiHandlerService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AdamServer.Interfaces.WebApiHandlerService
{
    public interface IPythonHandler
    {
        public string ExecutePythonCommandAsync(PythonCommand command);
        public string StopExecutePythonCommandAsync();
    }

    public class PythonHandler : IPythonHandler
    {
        #region Services

        private readonly ILogger<PythonHandler> mLogger;
        private readonly ITcpPythonStreamClientService mTcpPythonStreamClientService;
        
        #endregion

        public PythonHandler(IServiceProvider serviceProvider) 
        {
            mLogger = serviceProvider.GetRequiredService<ILogger<PythonHandler>>();
            mTcpPythonStreamClientService = serviceProvider.GetRequiredService<ITcpPythonStreamClientService>();
        }
        public string ExecutePythonCommandAsync(PythonCommand command)
        {
            File.WriteAllText("C:\\Users\\Professional\\Downloads\\python-3.13.0-embed-amd64\\test2.py", command.TextCommand);
            Task.Run(() => mTcpPythonStreamClientService.Connect());
            return "Ok";
            
        }

        public string StopExecutePythonCommandAsync()
        {
            mTcpPythonStreamClientService.Disconnect();
            return "Ok";
        }
    }
}
