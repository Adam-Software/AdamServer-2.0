using AdamServer.Interfaces.WebApiHandlerService;

namespace AdamServer.Interfaces
{
    public interface IWebApiHandlerService
    {
        public IPythonHandler PythonHandler { get; }
    }
}
