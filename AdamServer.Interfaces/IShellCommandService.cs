using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface IShellCommandService
    {
        public void ExecuteCommandAsync(string command);

        public Task ExecuteAndDebugCommandAsync(string command);
    }
}
