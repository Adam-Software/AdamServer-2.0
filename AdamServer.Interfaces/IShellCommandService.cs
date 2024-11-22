using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    public interface IShellCommandService
    {
        Task<string> ExecuteCommandAsync(string command);
    }
}
