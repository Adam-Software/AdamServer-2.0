using AdamServer.Interfaces;
using Chell;
using System.Text;
using static Chell.Exports;

namespace AdamServer.Services.Common
{
    public class ShellCommandService : IShellCommandService
    {
        public ShellCommandService() 
        {
            Env.Shell.UseCmd();
            Env.Verbosity = ChellVerbosity.Silent;
            
        }
        public async Task<string> ExecuteCommandAsync(string command)
        {
            Encoding systemEncoding = Console.OutputEncoding;
            ProcessOutput results = await Run(command, new ProcessTaskOptions(workingDirectory: @"C:\Windows", timeout: TimeSpan.FromSeconds(1)))
                .NoThrow()
                .SuppressConsoleOutputs();

            var bytes = results.OutputBinary.ToArray();
            var resultString = systemEncoding.GetString(bytes);

            return resultString;
        }
    }
}
