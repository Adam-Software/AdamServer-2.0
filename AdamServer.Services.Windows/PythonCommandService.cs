using AdamServer.Interfaces;
using AdamServer.Interfaces.PythonCommandServiceDependency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdamServer.Services.Windows
{
    public class PythonCommandService : IPythonCommandService
    {
        #region Events

        public event ProcessAndOutputEndedEventHandler RaiseProcessAndOutputEndedEvent;

        public event ProcessDataAndErrorOutputEventHandler RaiseProcessDataAndErrorOutput;

        #endregion

        #region Var

        private readonly ILogger<PythonCommandService> mLoogerService;
        
        private Process mProcess;

        private bool mIsOutputEnded;

        private bool mIsProcessEnded;

        #endregion

        #region ~

        public PythonCommandService(IServiceProvider serviceProvider)
        {
            mLoogerService = serviceProvider.GetRequiredService<ILogger<PythonCommandService>>();
        }

        #endregion

        #region Public fields

        public bool HasExited => mProcess.HasExited;

        #endregion

        #region Public methods


        public Task ExecuteCommandAsync(CancellationToken stoppingToken = default)
        {
            StartProcess(withDebug: false);
            return ProcessLiveCicle(stoppingToken);
        }

        public Task ExecuteAndDebugCommandAsync(CancellationToken stoppingToken = default)
        {
            StartProcess(withDebug: true);
            return ProcessLiveCicle(stoppingToken);
        }

        public void WriteLineToProcessStandartInput(string value)
        {
            if(!mProcess.HasExited)
                mProcess.StandardInput.WriteLine(value);
        }

        public void WriteDataToProcessStandartInpu(byte[] data)
        {
            string result = Console.OutputEncoding.GetString(data);

            if (!string.IsNullOrEmpty(result)) 
                WriteLineToProcessStandartInput(result);
        }

        public void CloseProcess()
        {
            mIsOutputEnded = true;
            mIsProcessEnded = true;
        }

        #endregion

        #region Private methods

        private void StartProcess(bool withDebug = false)
        {
            string arg = string.Format("-u -m test2");

            if (withDebug)
                arg = string.Format("-u -m pdb test2.py");

            ProcessStartInfo proccesInfo = new()
            {
                FileName = "C:\\Users\\Professional\\Downloads\\python-3.13.0-embed-amd64\\python.exe",
                WorkingDirectory = "C:\\Users\\Professional\\Downloads\\python-3.13.0-embed-amd64\\",
                Arguments = arg,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            mProcess = new()
            {
                StartInfo = proccesInfo,
                EnableRaisingEvents = true,
            };

            mProcess.Exited += ProcessExited;
            mProcess.OutputDataReceived += ProcessOutputDataReceived;
            mProcess.ErrorDataReceived += ProcessErrorDataReceived;
            mProcess.Start();

            mProcess.BeginOutputReadLine();
            mProcess.BeginErrorReadLine();
            //???
            mProcess.WaitForExit();
            //mProcess.Close();
        }

        private Task ProcessLiveCicle(CancellationToken stoppingToken = default)
        {
            return Task.Run(() =>
            {
                while (mIsOutputEnded && mIsProcessEnded)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    mIsOutputEnded = false;
                    mIsProcessEnded = false;

                    if (!mProcess.HasExited)
                    {
                        mProcess.CancelErrorRead();
                        mProcess.CancelOutputRead();

                        mProcess.Close();
                    }
                }

            });
        }

        private void OnProcessDataAndErrorOutput(string data)
        {
            OnRaiseProcessDataAndErrorOutput(data);
        }

        #endregion

        #region Console events

        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                mIsOutputEnded = true;
                return;    
            }

            OnProcessDataAndErrorOutput(e.Data);
            mIsOutputEnded = false;
        }

        private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                mIsOutputEnded = true;
                return;
            }

            OnProcessDataAndErrorOutput(e.Data);
            mIsOutputEnded = false;
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            ExitDataEventArgs exitDataEventArgs = new ExitDataEventArgs
            {
                 ExitCode = mProcess.ExitCode,
                 ExitTime = mProcess.ExitTime,
                 MachineName = mProcess.MachineName,
                 StartTime = mProcess.StartTime,
                 TotalProcessorTime = mProcess.TotalProcessorTime,
                 UserProcessorTime = mProcess.UserProcessorTime,

            };

            OnRaiseProcessAndOutputEndedEvent(exitDataEventArgs);
            mIsProcessEnded = true;
        }

        #endregion

        #region Dispose

        bool mDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                mProcess.Dispose();
            }

            mDisposed = true;
        }

        #endregion

        #region RaiseEvents

        protected virtual void OnRaiseProcessAndOutputEndedEvent(ExitDataEventArgs eventArgs)
        {
            var exit = eventArgs;
            ProcessAndOutputEndedEventHandler raiseEvent = RaiseProcessAndOutputEndedEvent;
            raiseEvent?.Invoke(this);
        }

        protected virtual void OnRaiseProcessDataAndErrorOutput(string data)
        {
            ProcessDataAndErrorOutputEventHandler raiseEvent = RaiseProcessDataAndErrorOutput;
            raiseEvent?.Invoke(this, data);
        }

        #endregion

    }
}
