using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface IShellCommandService
    {
        public Task<string> ExecuteCommandAsync(string command);
    }
}
