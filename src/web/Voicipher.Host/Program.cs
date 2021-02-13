using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using VoicipherHost = Microsoft.Extensions.Hosting.Host;

namespace Voicipher.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"Logs/logs.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]|[{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    fileSizeLimitBytes: 104857600,
                    rollingInterval: RollingInterval.Month,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            try
            {
                Log.ForContext<Program>().Information("Starting application up.");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>().Fatal(ex, "Application start-up failed.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            VoicipherHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("wwwroot");
                });
    }
}
