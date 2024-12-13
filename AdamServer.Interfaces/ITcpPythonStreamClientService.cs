using System.Threading;
using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface ITcpPythonStreamClientService
    {
        public Task ExecuteAsync(CancellationToken stoppingToken = default);
        public void StopAsync();
    }
}
