using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[Route("/fib")]
[ApiController]
public class FibonacciController : ControllerBase
{
    private readonly ActivitySource _activitySource;
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _projectDomain = Environment.GetEnvironmentVariable("PROJECT_DOMAIN");

    public FibonacciController(ActivitySource activitySource, IHttpClientFactory clientFactory)
    {
        _activitySource = activitySource;
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public async Task<int> CalculateFibonacciAsync(int index = 0)
    {
        // CUSTOM ATTRIBUTES (2 lines of code to uncomment)
        // using var span = _activitySource.StartActivity();
        // span.AddTag("√", index);

        if (index == 0 | index == 1)
            return 0;
        if (index == 2)
            return 1;

        var resOne = await GetNext(index - 1);
        var resTwo = await GetNext(index - 2);
        var fibonacciNumber = resOne + resTwo;
        AddResultSpan(fibonacciNumber);
        return fibonacciNumber;
    }

    private void AddResultSpan(int fibonacciNumber) {
        // using var activity = _activitySource.StartActivity("calculation");
        // activity.AddTag("result", fibonacciNumber);
    }

    private async Task<int> GetNext(int iv)
    {
        var client = _clientFactory.CreateClient();
        var resp = await client.GetAsync($"http://localhost:5000/fib?index={iv}");
        resp.EnsureSuccessStatusCode();
        return int.Parse(await resp.Content.ReadAsStringAsync());
    }
}
