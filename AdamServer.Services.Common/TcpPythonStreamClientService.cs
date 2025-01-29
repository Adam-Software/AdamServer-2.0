using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PHS.Networking.Enums;
using System.Diagnostics;
using System.Text;
using Tcp.NET.Client;
using Tcp.NET.Client.Events.Args;
using Tcp.NET.Client.Models;

namespace AdamServer.Services.Common
{
    public class TcpPythonStreamClientService : ITcpPythonStreamClientService, IDisposable
    {
        #region Var

        private readonly ILogger<TcpPythonStreamClientService> mLoggerService;
        private readonly TcpNETClient mTcpClient;
        //private readonly Encoding mSystemEncoding = Console.OutputEncoding;
        private Process mProcess;

        private bool mIsOutputEnded = false;
        private bool mIsProcessEnded = false;
        

        #endregion

        #region ~

        public TcpPythonStreamClientService(IServiceProvider serviceProvider) 
        {
            mLoggerService = serviceProvider.GetRequiredService<ILogger<TcpPythonStreamClientService>>();
            mTcpClient = new TcpNETClient(new ParamsTcpClient("127.0.0.1", 18000, "\r\n", isSSL: false));

            Subscribe();
        }

        #endregion

        #region Subscribe/Unsubscribe

        private void Subscribe()
        {
            mTcpClient.ConnectionEvent += ConnectionEvent;
            mTcpClient.MessageEvent += MessageEvent;
            mTcpClient.ErrorEvent += ErrorEvent;
        }

        private void UnSubscribe()
        {
            mTcpClient.ConnectionEvent -= ConnectionEvent;
            mTcpClient.MessageEvent -= MessageEvent;
            mTcpClient.ErrorEvent -= ErrorEvent;
        }

        #endregion

        #region Events

        private void ConnectionEvent(object sender, TcpConnectionClientEventArgs args)
        {
            var connectionEvent = args.ConnectionEventType;

            if (connectionEvent == ConnectionEventType.Connected) 
            {
                StartProcess(false);
            }

            if(connectionEvent == ConnectionEventType.Disconnect)
            {
                mIsOutputEnded = true;
                mIsProcessEnded = true;

                if (!mProcess.HasExited)
                {
                    mProcess.CancelErrorRead();
                    mProcess.CancelOutputRead();

                    mProcess.Close();
                }
            }
        }

        private void MessageEvent(object sender, TcpMessageClientEventArgs args)
        {
            var messageEvent = args.MessageEventType;

            if(messageEvent == MessageEventType.Receive)
            {
                var message = args.Message;
                if (string.IsNullOrEmpty(message))
                    mProcess.StandardInput.WriteLine(message);
            }
        }

        private void ErrorEvent(object sender, TcpErrorClientEventArgs args)
        {
         
        }

        #endregion

        #region Public field

        public Task ConnectAsync()
        {
            try
            {
                mTcpClient.ConnectAsync();
            }
            catch(Exception ex)
            {
                mLoggerService.LogError("TcpSockerClient connected error: {error}", ex.Message);
            }

            return Task.Run(() =>
            {
                while (mIsOutputEnded && mIsProcessEnded)
                {
                    mIsOutputEnded = false;
                    mIsProcessEnded = false;
                    mTcpClient.DisconnectAsync();
                }
            });
        }

        public Task DisconnectAsync()
        {
            try
            {
                mTcpClient.DisconnectAsync();
            }
            catch (Exception ex)
            {
                mLoggerService.LogError("TcpSockerClient disconnect error: {error}", ex.Message);
            }
     

            return Task.CompletedTask;
            
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
            mProcess.WaitForExit();
        }

        private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                try
                {
                    mTcpClient.SendAsync(string.Join("\n", e.Data));
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
                    mTcpClient.SendAsync($"{e.Data}\n");
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
