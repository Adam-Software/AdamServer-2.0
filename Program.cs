using AdamServer.Core;
using AdamServer.Core.Mappings;
using AdamServer.Interfaces;
using AdamServer.Interfaces.WebApiHandlerService;
using AdamServer.Services.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args); //Host.CreateApplicationBuilder(args);
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
            builder.Services.AddSingleton<IShellCommandService, ShellCommandService>();
            builder.Services.AddSingleton<IWebApiHandlerService, WebApiHandlerService>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                //builder.Services.AddSingleton<IService>(new LinuxService());
                //builder.Services.AddHostedService(serviceProvider => new LinuxHostedService(serviceProvider.GetService<IService>()));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //builder.Services.AddSingleton<IService>(new WindowsService());
                //builder.Services.AddHostedService(serviceProvider => new WindowsHostedService(serviceProvider.GetService<IService>()));
            }

            builder.Services.AddHostedService<ProgramHostedService>();

            //using IHost host = builder.Build();
            //await host.RunAsync();

            var app = builder.Build();

            PythonMapping.Map(app);
            
            await app.RunAsync();
        }
    }
}
