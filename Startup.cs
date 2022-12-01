using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;
using System.Collections.Generic;
using System;
using System.Collections.Immutable;

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

            OpenTelemetry.Sdk.SetDefaultTextMapPropagator(new IgnoreTraceHeadersOnForwardedRequests(Propagators.DefaultTextMapPropagator));
            // Configure Honeycomb using Configuration
            var honeycombOptions = Configuration.GetHoneycombOptions();
            services.AddOpenTelemetryTracing(otelBuilder =>
                otelBuilder
                .AddHoneycomb(honeycombOptions)
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
