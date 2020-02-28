using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenTracing;

namespace Iserv.IdentityServer4.BusinessLogic.Extension
{
    public class TracingRequestFilter : Microsoft.AspNetCore.Mvc.Filters.IResourceFilter
    {

        private readonly ITracer _tracer;

        public TracingRequestFilter(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void OnResourceExecuting(Microsoft.AspNetCore.Mvc.Filters.ResourceExecutingContext context)
        {
            var request = context.HttpContext.Request;

            var header = string.Join("; ", request.Headers.Select(h => h.Key + "=" + string.Join(", ", h.Value)));
            _tracer.ActiveSpan.SetTag("http.request.header", header);

            var body = GetBody(context.HttpContext);
            _tracer.ActiveSpan.SetTag("http.request.body", body);
        }

        public void OnResourceExecuted(Microsoft.AspNetCore.Mvc.Filters.ResourceExecutedContext context)
        {
            var response = context.HttpContext.Response;

            var header = string.Join("; ", response.Headers.Select(h => h.Key + "=" + string.Join(", ", h.Value)));
            _tracer.ActiveSpan.SetTag("http.response.header", header);

            switch (context.Result)
            {
                case ObjectResult result:
                    var json = JsonConvert.SerializeObject(result.Value);
                    _tracer.ActiveSpan.SetTag("http.response.result", json);
                    break;
            }
        }

        private static string GetBody(Microsoft.AspNetCore.Http.HttpContext context)
        {
            context.Request.EnableBuffering();
            var rd = context.Request.BodyReader.ReadAsync().Result;
            context.Request.Body.Position = 0;
            var buffer = rd.Buffer;
            var body = Encoding.UTF8.GetString(buffer.FirstSpan);
            context.Request.Body.Position = 0;
            return body;
        }
    }
}
