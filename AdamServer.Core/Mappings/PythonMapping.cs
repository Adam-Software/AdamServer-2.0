using AdamServer.Interfaces.WebApiHandlerService;
using AdamServer.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AdamServer.Core.Mappings
{
    public static class PythonMapping
    {

        private const string cApiPath = "PythonCommand";

        public static void Map(WebApplication webApplication)
        {
            IWebApiHandlerService handler = webApplication.Services.GetRequiredService<IWebApiHandlerService>();

            webApplication.MapPost($"/api/{cApiPath}/ExecuteAsync/", (PythonCommand command) => handler.ExecutePythonCommandAsync(command));
            webApplication.MapPut($"/api/{cApiPath}/StopExecuteAsync/", () => handler.StopExecutePythonCommandAsync());
        }
    }
}
