using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface ITcpPythonStreamClientService
    {
        public Task ConnectAsync();
        public Task DisconnectAsync();
    }
}
