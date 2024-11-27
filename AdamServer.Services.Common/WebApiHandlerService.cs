using AdamServer.Interfaces;
using AdamServer.Interfaces.WebApiHandlerService;

namespace AdamServer.Services.Common
{
    public class WebApiHandlerService : IWebApiHandlerService
    {
        #region Services

        private readonly IServiceProvider mServiceProvider;
        
        #endregion

        public WebApiHandlerService(IServiceProvider serviceProvider)
        {
            mServiceProvider = serviceProvider;
        }

        public IPythonHandler PythonHandler => new PythonHandler(mServiceProvider);
    }
}
