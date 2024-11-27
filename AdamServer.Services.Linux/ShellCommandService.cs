using AdamServer.Interfaces;
using System.Threading.Tasks;

namespace AdamServer.Services.Linux
{
    public class ShellCommandService : IShellCommandService
    {
        public Task<string> ExecuteCommandAsync(string command)
        {
            throw new System.NotImplementedException();
        }
    }
}
