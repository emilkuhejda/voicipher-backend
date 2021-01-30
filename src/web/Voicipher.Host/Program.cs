using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VoicipherHost = Microsoft.Extensions.Hosting.Host;

namespace Voicipher.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            VoicipherHost.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
