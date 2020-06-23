using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace SecureWebApp
{

    public class Program
    {

        public static IWebHostBuilder CreateWebHostBuilder(string[] Args) =>
            WebHost.CreateDefaultBuilder(Args)
                .UseStartup<Startup>()
                .UseSerilog();

        public static int Main(string[] Args)
        {

            var LogsPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(LogsPath))
            {
                Directory.CreateDirectory(LogsPath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    LogsPath + "\\log-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: null,
                    shared: false)
                .CreateLogger();

            try
            {
                Log.Information("Starting WebHost...");
                CreateWebHostBuilder(Args).Build().Run();
                return 0;
            }
            catch (Exception E)
            {
                Log.Fatal(E, "WebHost has been terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

    }

}
