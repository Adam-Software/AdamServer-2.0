using AdamServer.Core;
using AdamServer.Core.Mappings;
using AdamServer.Interfaces;
using AdamServer.Services.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace AdamServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args); 
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            AppSettingsOptionsService options = new();
            builder.Configuration.GetRequiredSection("AppSettingsOptions").Bind(options);
            builder.Services.AddSingleton<IAppSettingsOptionsService>(options);

            Logger mainLogger = new LoggerConfiguration()
                .WriteTo.File("logs/log-.txt",
                            rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .CreateLogger();


            builder.Services.AddLogging(s => s.AddSerilog(mainLogger, dispose: true));

            /*Obsolete AuthenticationScheme*/
            //builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("", options => { });

            builder.Services.AddSingleton<IWebApiHandlerService, WebApiHandlerService>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                builder.Services.AddSingleton<IShellCommandService, Services.Linux.ShellCommandService>();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                builder.Services.AddSingleton<IShellCommandService, Services.Windows.ShellCommandService>();
            }

            builder.Services.AddHostedService<ProgramHostedService>();
            
            var app = builder.Build();

            PythonMapping.Map(app);
            
            await app.RunAsync();
        }
    }
}
