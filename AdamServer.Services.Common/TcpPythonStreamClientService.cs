using AdamServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PHS.Networking.Enums;
using System.Diagnostics;
using System.Text;
using Tcp.NET.Client;
using Tcp.NET.Client.Events.Args;
using Tcp.NET.Client.Models;
using TcpSharp;

namespace AdamServer.Services.Common
{
    public class TcpPythonStreamClientService : ITcpPythonStreamClientService, IDisposable
    {
        #region Var

        private readonly ILogger<TcpPythonStreamClientService> mLoggerService;
        private readonly IPythonCommandService mPythonCommandService;
        private readonly TcpNETClient mTcpClient;
        private CancellationTokenSource mToken;


        #endregion

        #region ~

        public TcpPythonStreamClientService(IServiceProvider serviceProvider) 
        {
            mLoggerService = serviceProvider.GetRequiredService<ILogger<TcpPythonStreamClientService>>();
            mPythonCommandService = serviceProvider.GetRequiredService<IPythonCommandService>();

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

            mPythonCommandService.RaiseProcessAndOutputEndedEvent += RaiseProcessAndOutputEndedEvent;
            mPythonCommandService.RaiseProcessDataAndErrorOutput += RaiseProcessDataAndErrorOutput;
        }

        private void UnSubscribe()
        {
            mTcpClient.ConnectionEvent -= ConnectionEvent;
            mTcpClient.MessageEvent -= MessageEvent;
            mTcpClient.ErrorEvent -= ErrorEvent;
            
            mPythonCommandService.RaiseProcessAndOutputEndedEvent -= RaiseProcessAndOutputEndedEvent;
            mPythonCommandService.RaiseProcessDataAndErrorOutput -= RaiseProcessDataAndErrorOutput;
        }

        #endregion

        #region Events

        private void ConnectionEvent(object sender, TcpConnectionClientEventArgs args)
        {
            var connectionEvent = args.ConnectionEventType;

            if (connectionEvent == ConnectionEventType.Connected) 
            {
                mToken = new CancellationTokenSource();
                mLoggerService.LogInformation("Client connected to server");
                mPythonCommandService.ExecuteCommandAsync(mToken.Token);
            }

            if(connectionEvent == ConnectionEventType.Disconnect)
            {
                mLoggerService.LogInformation("Client disconnected");
                
                if(!mPythonCommandService.HasExited) 
                    mPythonCommandService.CloseProcess();

                mToken = null;
            }
        }

        private void MessageEvent(object sender, TcpMessageClientEventArgs args)
        {
            var messageEvent = args.MessageEventType;
            
            if(messageEvent == MessageEventType.Receive) 
                mPythonCommandService.WriteDataToProcessStandartInpu(args.Bytes);
        }

        private void ErrorEvent(object sender, TcpErrorClientEventArgs args)
        {
         
        }


        //when process done, client disconect
        private void RaiseProcessAndOutputEndedEvent(object sender)
        {
            mTcpClient.DisconnectAsync();
        }

        private void RaiseProcessDataAndErrorOutput(object sender, string data)
        {
            try
            {
                mTcpClient.SendAsync($"{data}\n");
            }
            catch (Exception ex) 
            {
                mLoggerService.LogError("Error send data: {error}", ex.Message);
            }
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

            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            try
            {
                //mToken.Cancel();
                //mPythonCommandService.CloseProcess();
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


        #endregion

        public void Dispose()
        {
            UnSubscribe();
        }
    }
}
