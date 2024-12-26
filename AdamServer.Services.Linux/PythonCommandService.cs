using AdamServer.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace AdamServer.Services.Linux
{
    public class PythonCommandService : IPythonCommandService
    {
        public bool HasExited => throw new System.NotImplementedException();

        public event ProcessAndOutputEndedEventHandler RaiseProcessAndOutputEndedEvent;
        public event ProcessDataAndErrorOutputEventHandler RaiseProcessDataAndErrorOutput;

        public void CloseProcess()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task ExecuteAndDebugCommandAsync(CancellationToken stoppingToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task ExecuteCommandAsync(CancellationToken stoppingToken = default)
        {
            throw new System.NotImplementedException();
        }

        public void WriteDataToProcessStandartInpu(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public void WriteLineToProcessStandartInput(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
