using AdamServer.Interfaces.WebApiHandlerService;

namespace AdamServer.Interfaces
{
    public interface IWebApiHandlerService
    {
        public IPythonHandler PythonHandler { get; }

        //public Task<string> ExecutePythonCommandAsync(PythonCommand command);
        //public Task StopExecutePythonCommandAsync();
    }
}
