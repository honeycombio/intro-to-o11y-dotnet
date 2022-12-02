using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using intro_to_observability_dotnet;
using Microsoft.AspNetCore.Mvc;

public class FibonacciController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public FibonacciController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("/fib")]
    public async Task<int> CalculateFibonacciAsync(int index = 0)
    {
        // CUSTOM ATTRIBUTES (1 lines of code to uncomment)
        // Activity.Current.SetTag("parameter.index", index);

        if (index == 0 | index == 1)
            return 0;
        if (index == 2)
            return 1;

        var resOne = await GetNext(index - 1);
        var resTwo = await GetNext(index - 2);
        var fibonacciNumber = resOne + resTwo;

        // CUSTOM ATTRIBUTES (2 lines of code to uncomment)
        // using var calculationActivity = ActivityConfig.Source.StartActivity("calculation");
        // calculationActivity.SetTag("result", fibonacciNumber);
        return fibonacciNumber;
    }

    private async Task<int> GetNext(int iv)
    {
        var client = _clientFactory.CreateClient();
        var resp = await client.GetAsync($"http://localhost:5000/fib?index={iv}");
        resp.EnsureSuccessStatusCode();
        return int.Parse(await resp.Content.ReadAsStringAsync());
    }
}
