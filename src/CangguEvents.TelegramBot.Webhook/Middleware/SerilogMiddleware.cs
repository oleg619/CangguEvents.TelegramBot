using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace CangguEvents.TelegramBot.Webhook.Middleware
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers",
                "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name");
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");
            return _next(httpContext);
        }
    }

    internal class SerilogMiddleware
    {
        private const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private static readonly ILogger Log = Serilog.Log.ForContext<SerilogMiddleware>();

        private readonly RequestDelegate _next;

        public SerilogMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var sw = Stopwatch.StartNew();

            try
            {
                await _next(httpContext);
                sw.Stop();

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;

                log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path,
                    statusCode, sw.Elapsed.TotalMilliseconds);
            }
            // Never caught, because `LogException()` returns false.
            // catch (Exception ex) when (LogException(httpContext, sw, ex))
            catch (Exception ex) when (LogException(httpContext, sw, ex))
            {
            }
        }

        private static bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex)
        {
            sw.Stop();

            LogForErrorContext(httpContext)
                .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500,
                    sw.Elapsed.TotalMilliseconds);

            return false;
        }

        private static ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
            {
                result = result.ForContext("RequestForm",
                    request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));
            }

            return result;
        }
    }
}