using AdamServer.Interfaces;
using static Chell.Exports;

namespace AdamServer.Services.Common
{
    public class ShellCommandService : IShellCommandService
    {
        public async Task<string> ExecuteCommandAsync(string command)
        {
            Chell.ProcessOutput results = await Run($"{command}")
                .NoThrow()
                .SuppressConsoleOutputs();


            return results.Output;
        }
    }
}
