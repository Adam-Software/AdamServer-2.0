using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface IShellCommandService
    {
        public Task ExecuteCommandAsync(string command);

        public Task ExecuteAndDebugCommandAsync(string command);
    }
}
