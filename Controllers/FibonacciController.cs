using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OpenTelemetry.Trace;

namespace fib.Controllers
{
    [ApiController]
    [Route("/fib")]
    public class FibonacciController : ControllerBase
    {
        private readonly Tracer _tracer;
        private readonly IHttpClientFactory _clientFactory;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _accessor;

        public FibonacciController(Tracer tracer, IHttpClientFactory clientFactory, LinkGenerator linkGenerator, IHttpContextAccessor accessor)
        {
            _tracer = tracer;
            _clientFactory = clientFactory;
            _linkGenerator = linkGenerator;
            _accessor = accessor;
        }

        [HttpGet]
        public async Task<int> CalculateFibonacciAsync(int index = 0)
        {
            var iv = index;
            using (var span = _tracer.StartActiveSpan("fibonacci"))
            {
                span.SetAttribute("parameter.index", iv);

                if (iv == 0)
                    return 0;
                if (iv == 1)
                    return 1;

                var result = await GetNext(iv - 1) + await GetNext(iv - 2);
                span.SetAttribute("result", result);
                return result;
            }
        }

        private async Task<int> GetNext(int iv)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var url = this._linkGenerator.GetUriByPage(this._accessor.HttpContext, page: null, handler: null);
                var resp = await client.GetAsync($"{url}?index={iv}");
                resp.EnsureSuccessStatusCode();
                return int.Parse(await resp.Content.ReadAsStringAsync());
            }
        }
    }
}