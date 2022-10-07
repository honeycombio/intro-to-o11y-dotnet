using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

[Route("/fib")]
[ApiController]
public class FibonacciController : ControllerBase
{
    private readonly Tracer _tracer;
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _projectDomain = Environment.GetEnvironmentVariable("PROJECT_DOMAIN");
    private readonly ILogger _logger;

    public FibonacciController(Tracer tracer, IHttpClientFactory clientFactory, ILogger<FibonacciController> logger)
    {
        _tracer = tracer;
        _clientFactory = clientFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<int> CalculateFibonacciAsync(int index = 0)
    {
        _logger.LogInformation("start calculate fib {index}.", index);

        // CUSTOM ATTRIBUTES (2 lines of code to uncomment)
        var currentSpan = Tracer.CurrentSpan;
        currentSpan.SetAttribute("parameter.index", index);

        if (index == 0 | index == 1)
            return 0;
        if (index == 2)
            return 1;

        var resOne = await GetNext(index - 1);
        var resTwo = await GetNext(index - 2);

        // CUSTOM SPAN (3 sections of code to uncomment, 4 lines total)
        using (var span = _tracer.StartActiveSpan("calculation"))
        {
           var fibonacciNumber = resOne + resTwo;
           span.SetAttribute("result", fibonacciNumber);
           _logger.LogInformation("done calculate fib {result}.", fibonacciNumber);
           return fibonacciNumber;
        };
    }

    private async Task<int> GetNext(int iv)
    {
        var client = _clientFactory.CreateClient();
        var resp = await client.GetAsync($"http://localhost:5000/fib?index={iv}");
        resp.EnsureSuccessStatusCode();
        return int.Parse(await resp.Content.ReadAsStringAsync());
    }
}
