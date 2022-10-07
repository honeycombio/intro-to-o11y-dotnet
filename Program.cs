using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using System;
using System.Net.Http;

namespace intro_to_observability_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureLogging(builder =>
                 {
                     var appResourceBuilder = ResourceBuilder.CreateDefault()
                         .AddService("fib-microsvc-log", "0.0.1");

                     builder.AddOpenTelemetry(options =>
                     {
                         options.SetResourceBuilder(appResourceBuilder);
                         options.AddOtlpExporter(option =>
                         {
                             option.Endpoint = new Uri("https://api.honeycomb.io");
                             option.Headers = "x-honeycomb-team=xxxxx";
                         });
                     });
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
