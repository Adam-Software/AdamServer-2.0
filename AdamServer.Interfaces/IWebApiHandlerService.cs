using AdamServer.Interfaces.WebApiHandlerService;
using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface IWebApiHandlerService
    {
        public Task<string> ExecutePythonCommandAsync(PythonCommand command);
        public Task StopExecutePythonCommandAsync();
    }
}
