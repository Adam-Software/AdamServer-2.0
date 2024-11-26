using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface IWebApiHandlerService
    {
        public Task<string> ExecuteCommandAsync(string command);
    }
}
