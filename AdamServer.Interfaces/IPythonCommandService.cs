using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdamServer.Interfaces
{
    #region Delegates

    public delegate void ProcessAndOutputEndedEventHandler(object sender);

    public delegate void ProcessDataAndErrorOutputEventHandler(object sender, string data);

    #endregion

    public interface IPythonCommandService : IDisposable
    {
        #region Event

        public event ProcessAndOutputEndedEventHandler RaiseProcessAndOutputEndedEvent;

        public event ProcessDataAndErrorOutputEventHandler RaiseProcessDataAndErrorOutput;

        #endregion


        #region Fields

        //public bool IsOutputEnded { get; }
        //public bool IsProcessEnded { get; }
        //public Encoding SystemOutputEncoding { get; }

        public bool HasExited { get; }

        #endregion

        #region Methods

        public Task ExecuteCommandAsync(CancellationToken stoppingToken = default);

        public Task ExecuteAndDebugCommandAsync(CancellationToken stoppingToken = default);

        public void WriteLineToProcessStandartInput(string value);

        public void WriteDataToProcessStandartInpu(byte[] data);

        public void CloseProcess();

        #endregion
    }
}
