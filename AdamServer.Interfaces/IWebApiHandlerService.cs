using AdamServer.Interfaces.WebApiHandlerService.Models;
using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface IWebApiHandlerService
    {
        public Task<string> ExecutePythonCommandAsync(PythonCommand command);
        public Task StopExecutePythonCommandAsync();
    }
}
