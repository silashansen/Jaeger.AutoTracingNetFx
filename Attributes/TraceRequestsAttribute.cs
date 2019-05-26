using Jaeger;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using OpenTracing.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApplication1.Attributes
{

    public class TraceRequestsAttribute : ActionFilterAttribute
    {
        private const string TraceContextKey = "__ot.trace__";
        private ITracer _tracer;


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(_tracer == null)
            {
                var requestScope = actionContext.Request.GetDependencyScope();
                var service = requestScope.GetService(typeof(ITracer)) as ITracer;
                _tracer = service;
            }


            base.OnActionExecuting(actionContext);
            var request = actionContext.Request;

            var spanBuilder = _tracer.BuildSpan(actionContext.Request.RequestUri.LocalPath);

            var headers = request.Headers.ToDictionary(k => k.Key, v => v.Value.First());
            ISpanContext parentSpanCtx = _tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(headers));
            if(parentSpanCtx != null)
            {
                spanBuilder.AsChildOf(parentSpanCtx);
            }

            var scope =
            spanBuilder
                .WithTag(Tags.SpanKind, Tags.SpanKindClient)
                .WithTag(Tags.HttpMethod, request.Method.ToString())
                .WithTag(Tags.HttpUrl, request.RequestUri.ToString())
                .WithTag(Tags.PeerHostname, request.RequestUri.Host)
                .WithTag(Tags.PeerPort, request.RequestUri.Port)
                .StartActive(true);

            actionContext.Request.Properties.Add(TraceContextKey, scope);

        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            var scope = (IScope)actionExecutedContext.Request.Properties[TraceContextKey];
            
            var response = actionExecutedContext.Response;

            if (response != null)
            {
                scope.Span.SetTag(Tags.HttpStatus, (int)response.StatusCode);
            }

            scope.Dispose();

            actionExecutedContext.Request.Properties[TraceContextKey] = null;
        }




    }
}