using OpenTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly ITracer _tracer;

        public ValuesController(ITracer tracer)
        {

            _tracer = tracer;
        }

        // GET api/values
        public async Task<IEnumerable<string>> Get()
        {
            var rnd = new Random();

            await Task.Delay(rnd.Next(100, 1500));
            using (var scope = _tracer.BuildSpan("Return values").StartActive())
            {
                await Task.Delay(rnd.Next(100, 1500));
                using (var scope2 = _tracer.BuildSpan("Construct response").StartActive())
                {
                    await Task.Delay(rnd.Next(100, 1500));
                    return new string[] { "value1", "value2" };
                }
            }
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
