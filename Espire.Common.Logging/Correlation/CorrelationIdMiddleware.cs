using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Espire.Common.Logging.Correlation
{
    public class CorrelationIdMiddleware : IMiddleware
    {
       
        private const string _correlationIdHeader = "X-Correlation-Id";
        private readonly ICorrelationIdGenerator correlationIdGenerator;
        //private readonly ILogger<CorrelationIdMiddleware> logger;

        public CorrelationIdMiddleware(ICorrelationIdGenerator correlationIdGenerator)
        {
            this.correlationIdGenerator = correlationIdGenerator;
            //this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationId = GetCorrelationId(context, correlationIdGenerator);
            AddcorrelationIdHeader(context, correlationId);
           // using(var scope = logger.BeginScope("Creating correlation context for scoped request {correlationId}",correlationId))
            await next(context);
        }
        private StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            if (context.Request.Headers.TryGetValue(_correlationIdHeader, out var correlationId))
            {
                correlationIdGenerator.Set(correlationId);
                return correlationId;
            }
            else
            {
                context.TraceIdentifier = correlationId;
                return correlationIdGenerator.Get();
                
            }
        }

        private static void AddcorrelationIdHeader(HttpContext context, StringValues correlationId)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(_correlationIdHeader, new[] { correlationId.ToString() });
                return Task.CompletedTask;
            });
        }
    }
}
