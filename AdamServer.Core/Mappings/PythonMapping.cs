using AdamServer.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AdamServer.Interfaces.WebApiHandlerService.Models;

namespace AdamServer.Core.Mappings
{
    public static class PythonMapping
    {

        private const string cApiPath = "PythonCommand";

        public static void Map(WebApplication webApplication)
        {
            IWebApiHandlerService handler = webApplication.Services.GetRequiredService<IWebApiHandlerService>();

            webApplication.MapPost($"/api/{cApiPath}/ExecuteAsync/", (PythonCommand command) => handler.PythonHandler.ExecutePythonCommandAsync(command));
            webApplication.MapGet($"/api/{cApiPath}/StopExecuteAsync/", handler.PythonHandler.StopExecutePythonCommandAsync);
        }
    }
}
