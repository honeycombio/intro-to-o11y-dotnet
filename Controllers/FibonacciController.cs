using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using System.Net.Http;

namespace WebApplication.Controllers
{
    [Route("/fib")]
    [ApiController]
    public class FibonacciController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly String _projectDomain = Environment.GetEnvironmentVariable("PROJECT_DOMAIN");
        
        public FibonacciController(IHttpClientFactory clientFactory)
        {
          _clientFactory = clientFactory;
        }
        
        [HttpGet]
        public async Task<ActionResult<int>> CalculateFibonacci(int index = 0)
        {
            int iv = index;
            System.Diagnostics.Activity.Current.AddTag("parameter", iv.ToString());
          
            var client = _clientFactory.CreateClient();
            if (iv == 0) {
              return 0;
            } else if (iv == 1) {
              return 1;
            } else {
              var requestOne = new HttpRequestMessage(HttpMethod.Get, $"http://{_projectDomain}.glitch.me/fib/?index={iv - 1}");
              var requestTwo = new HttpRequestMessage(HttpMethod.Get, $"http://{_projectDomain}.glitch.me/fib/?index={iv - 2}");
              var resOne = await client.SendAsync(requestOne);
              var resTwo = await client.SendAsync(requestTwo);
              resOne.EnsureSuccessStatusCode();
              resTwo.EnsureSuccessStatusCode();
              var resultOne = await resOne.Content.ReadAsStringAsync();
              var resultTwo = await resTwo.Content.ReadAsStringAsync();
              var finalResult = Int32.Parse(resultOne) + Int32.Parse(resultTwo);
              return finalResult;
            }
        }
    }
}
