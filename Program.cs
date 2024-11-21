using AdamServer.Core;
using AdamServer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices;


namespace AdamServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            AppSettingsOptions options = new();
            builder.Configuration.GetRequiredSection("AppSettingsOptions").Bind(options);
            builder.Services.AddSingleton<IAppSettingsOptions>(options);

            try
            {
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

                builder.Services.AddHostedService(serviceProvider => new ProgramHostedService(serviceProvider));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            using IHost host = builder.Build();

            host.RunAsync().Wait();
            host.WaitForShutdownAsync();
        }
    }
}
