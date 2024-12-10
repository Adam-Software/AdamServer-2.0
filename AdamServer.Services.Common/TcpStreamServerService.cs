using AdamServer.Interfaces;
using CavemanTcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpSharp;

namespace AdamServer.Services.Common
{
    public class TcpStreamServerService : BackgroundService, IStreamServerService, IDisposable
    {
        private readonly ILogger<TcpStreamServerService> mLoggerService;
        private readonly TcpSharpSocketServer mServer;
        private readonly Encoding mSystemEncoding = Console.OutputEncoding;
        string mConnectedClientId = string.Empty;
        StreamWriter mProcessWriterInput = null;
        //StreamWriter streamWriter = new(Stream);
        #region ~

        public TcpStreamServerService(IServiceProvider serviceProvider) 
        {
            mLoggerService = serviceProvider.GetRequiredService<ILogger<TcpStreamServerService>>();

            mServer = new TcpSharpSocketServer(18000);
            Subscribe();
        }

        #endregion

        #region Subscribe/Unsubscribe

        private void Subscribe()
        {
            mServer.OnConnectionRequest += OnConnectionRequest;
            mServer.OnConnected += OnConnected;
            mServer.OnDataReceived += OnDataReceived;
            mServer.OnDisconnected += OnDisconnected;
        }

        private void OnDataReceived(object sender, OnServerDataReceivedEventArgs e)
        {
            var result = mSystemEncoding.GetString(e.Data);
            if (mConnectedClientId != null) 
            {
                mProcessWriterInput.WriteLine(result);
            }
        }

        private void UnSubscribe()
        {
            mServer.OnConnectionRequest -= OnConnectionRequest;
            mServer.OnConnected -= OnConnected;
            mServer.OnDisconnected -= OnDisconnected;
        }

        #endregion

        #region Events

        private void OnDisconnected(object sender, OnServerDisconnectedEventArgs e)
        {
            mLoggerService.LogInformation("Client {ConnectionId} disconnected", e.ConnectionId);
            mIsOutputEnded = true;
            mIsProcessEnded = true;
            mConnectedClientId = string.Empty;
            
            if(!mProcess.HasExited)
                mProcess.Kill();
        }


        private void OnConnected(object sender, OnServerConnectedEventArgs e)
        {
            mLoggerService.LogInformation("Cliet {guid} connected", e.ConnectionId);
            mConnectedClientId = e.ConnectionId;    
            Task.Run(RunCommand);
        }

        private void OnConnectionRequest(object sender, OnServerConnectionRequestEventArgs e)
        {
            mLoggerService.LogTrace("ConnectionRequest from {IPAddress}", e.IPAddress);

            if (string.IsNullOrEmpty(mConnectedClientId))
            {
                e.Accept = true;
                return;
            }
            
            e.Accept = false;
        }

        #endregion

        #region Execute start/stop

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            mServer.StartListening();
            mLoggerService.LogTrace("Stream server Listening = {Listening}", mServer.Listening);

            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    while (mIsOutputEnded && mIsProcessEnded)
                    {
                        mServer.Disconnect(mConnectedClientId);
                        mIsOutputEnded = false;
                        mIsProcessEnded = false;
                    }
                }
            });
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            mServer.StopListening();
            return base.StopAsync(cancellationToken);
        }

        #endregion

        #region Public field

        #endregion

        #region Private methods

        private bool mIsOutputEnded = false;
        private bool mIsProcessEnded = false;

        Process mProcess;

        private  void RunCommand()
        {
            ProcessStartInfo proccesInfo = new()
            {
                FileName = "C:\\Users\\Professional\\Downloads\\python-3.13.0-embed-amd64\\python.exe",
                WorkingDirectory = "C:\\Users\\Professional\\Downloads\\python-3.13.0-embed-amd64\\",

                //Arguments = string.Format("-u -m pdb test.py"),
                Arguments = string.Format("-u -m test"),
                UseShellExecute = false,
                CreateNoWindow = true, 
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            mProcess = new()
            {
                StartInfo = proccesInfo,
                EnableRaisingEvents = true
            };

            mProcess.Exited += ProcessExited;
            mProcess.OutputDataReceived += ProcessOutputDataReceived;
            mProcess.ErrorDataReceived += ProcessErrorDataReceived;
            mProcess.Start();

            mProcessWriterInput = mProcess.StandardInput;
            
            mProcess.BeginOutputReadLine();
            mProcess.BeginErrorReadLine();
            //mProcess.WaitForExit();
        }

        private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                try
                {
                    mServer.SendString(mConnectedClientId, $"{e.Data}\n");
                    mIsOutputEnded = false;
                }
                catch
                {
                    //mLoggerService.LogTrace("Error while output recived {exception}", ex);
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
                    mServer.SendString(mConnectedClientId, $"{e.Data}\n");
                    mIsOutputEnded = false;
                }
                catch 
                {
                    //mLoggerService.LogTrace("Error while output recived {exception}", ex);
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

        public override void Dispose()
        {
            UnSubscribe();
            base.Dispose();
        }
    }
}
