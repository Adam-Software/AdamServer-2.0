using AdamServer.Interfaces;
using System.Threading.Tasks;

namespace AdamServer.Services.Linux
{
    public class ShellCommandService : IShellCommandService
    {
        public void ExecuteAndDebugCommandAsync(string command)
        {
            throw new System.NotImplementedException();
        }

        public Task ExecuteCommandAsync(string command)
        {
            throw new System.NotImplementedException();
        }

        Task IShellCommandService.ExecuteAndDebugCommandAsync(string command)
        {
            throw new System.NotImplementedException();
        }

        void IShellCommandService.ExecuteCommandAsync(string command)
        {
            throw new System.NotImplementedException();
        }
    }
}
