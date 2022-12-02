using System.Collections.Generic;
using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace intro_to_observability_dotnet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHttpClient();
            services.AddSingleton(TracerProvider.Default.GetTracer("SomeTracer"));
            var honeycombConfig = Configuration.GetHoneycombOptions();
            services.AddSingleton(new ActivitySource(honeycombConfig.ServiceName));

            services.Configure<JaegerExporterOptions>(Configuration.GetSection("Jaeger"));
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(services.BuildServiceProvider().GetRequiredService<IOptions<JaegerExporterOptions>>().Value.Endpoint));
            OpenTelemetry.Sdk.SetDefaultTextMapPropagator(new IgnoreTraceHeadersOnForwardedRequests(Propagators.DefaultTextMapPropagator));
            // services.AddHttpClient("JaegerExporter");
            services.AddOpenTelemetryTracing(otelBuilder =>
                otelBuilder
                    .ConfigureResource(r => r.AddService("Samples.SampleClient"))
                    .AddSource("Samples.SampleClient", "Samples.SampleServer")
// Configure relevant auto instrumentation sections
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()

//  Configure exporters here, Honeycomb is configured by default, however other exporters are commented out
//                    .AddConsoleExporter()
//                    .AddJaegerExporter()
                    .AddHoneycomb(honeycombConfig)
            );
        }

        class IgnoreTraceHeadersOnForwardedRequests : TextMapPropagator
        {
            private TextMapPropagator _base;
            public IgnoreTraceHeadersOnForwardedRequests(TextMapPropagator otherPropagator)
            {
                _base = otherPropagator;
            }

            public override ISet<string> Fields
            {
                get
                {
                    var fields = _base.Fields;
                    fields.Add("x-forwarded-for");
                    return fields;
                }
            }

            public override PropagationContext Extract<T>(PropagationContext context, T carrier, Func<T, string, IEnumerable<string>> getter)
            {
                var xff = getter(carrier, "x-forwarded-for"); // what happens when it is not there?
                if (xff.ToImmutableList().IsEmpty)
                {
                    // header absent: use the standard extraction mechanism
                    return _base.Extract(context, carrier, getter);
                }
                else
                {
                    // header present: do not extract any trace information from the headers. Return the unmodified context
                    return context;
                }
            }

            public override void Inject<T>(PropagationContext context, T carrier, Action<T, string, string> setter)
            {
                _base.Inject(context, carrier, setter);
            }

            public override string ToString()
            {
                return "I AM THE WALRUS";
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // app.UseHttpsRedirection(); // removing this makes it work locally!
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
