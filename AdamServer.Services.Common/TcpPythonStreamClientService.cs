using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using TcpSharp;

namespace AdamServer.Services.Common
{
    public class TcpPythonStreamClientService : ITcpPythonStreamClientService, IDisposable
    {
        private readonly ILogger<TcpPythonStreamClientService> mLoggerService;
        private readonly TcpSharpSocketClient mTcpSocketClient;
        private readonly Encoding mSystemEncoding = Console.OutputEncoding;
        private bool mIsOutputEnded = false;
        private bool mIsProcessEnded = false;
        private Process mProcess;

        #region ~

        public TcpPythonStreamClientService(IServiceProvider serviceProvider) 
        {
            mLoggerService = serviceProvider.GetRequiredService<ILogger<TcpPythonStreamClientService>>();

            mTcpSocketClient = new TcpSharpSocketClient
            {
                Host = "127.0.0.1",
                KeepAlive = false,
                Port = 18000,
            };

            Subscribe();
        }

        #endregion

        #region Subscribe/Unsubscribe

        private void Subscribe()
        {
            mTcpSocketClient.OnConnected += OnConnected;
            mTcpSocketClient.OnDataReceived += OnDataReceived;
            mTcpSocketClient.OnDisconnected += OnDisconnected;
        }

        private void UnSubscribe()
        {
            mTcpSocketClient.OnConnected -= OnConnected;
            mTcpSocketClient.OnDataReceived -= OnDataReceived;
            mTcpSocketClient.OnDisconnected -= OnDisconnected;
        }

        #endregion

        #region Events

        private void OnConnected(object sender, OnClientConnectedEventArgs e)
        {
            mLoggerService.LogInformation("Clien connected to server {ServerIPAddress} port {port}", e.ServerIPAddress, e.ServerPort);
            
            StartProcess(false);
        }

        private void OnDataReceived(object sender, OnClientDataReceivedEventArgs e)
        {
            string result = mSystemEncoding.GetString(e.Data);

            if (string.IsNullOrWhiteSpace(result))
                mProcess.StandardInput.WriteLine(result);
        }

        private void OnDisconnected(object sender, OnClientDisconnectedEventArgs e)
        {
            mLoggerService.LogInformation("Client disconnected Reason {Reason}", e.Reason);

            mIsOutputEnded = true;
            mIsProcessEnded = true;

            if (!mProcess.HasExited)
            {
                mProcess.CancelErrorRead();
                mProcess.CancelOutputRead();

                mProcess.Kill();
            }

            //mConnectedClient = null;
        }

        #endregion

        #region Public field

        public Task ExecuteAsync(CancellationToken stoppingToken = default)
        {
            try
            {
                mTcpSocketClient.Connect();
            }
            catch(Exception ex)
            {
                mLoggerService.LogError("TcpSockerClient connected error: {error}", ex.Message);
                return Task.CompletedTask;
            }

            mLoggerService.LogTrace("TcpSockerClient connected is {Connected}", mTcpSocketClient.Connected);

            return Task.Run(() =>
            {
                while (mIsOutputEnded && mIsProcessEnded)
                {
                    mIsOutputEnded = false;
                    mIsProcessEnded = false;

                    mTcpSocketClient.Disconnect();
                }
                
            }, stoppingToken);
        }

        public void StopAsync()
        {
            mTcpSocketClient.Disconnect();
        }

        #endregion

        #region Private methods

        private  void StartProcess(bool withDebug = false)
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
            mProcess.WaitForExit();
        }

        private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                try
                {
                    mTcpSocketClient.SendString(string.Join("\n", e.Data), mSystemEncoding);
                    mIsOutputEnded = false;
                }
                catch
                {
                    mIsOutputEnded = true;
                }
                
            }
            else
            {
                mIsOutputEnded = true;
            }
        }

        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                try
                {
                    mTcpSocketClient.SendString($"{e.Data}\n", mSystemEncoding);
                    mIsOutputEnded = false;
                }
                catch 
                {
                    mIsOutputEnded = true;
                }
            }
            else
            {
                mIsOutputEnded = true;
            }
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            mIsProcessEnded = true;
        }

        #endregion

        public void Dispose()
        {
            UnSubscribe();
        }
    }
}
