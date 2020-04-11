using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace CangguEvents.TelegramBot.Webhook.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error("Something went wrong: {ex}", ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            var errorDetails = new ErrorDetails(context.Response.StatusCode,
                "Internal Server Error from the custom middleware.");

            var errorMessage = JsonConvert.SerializeObject(errorDetails, Formatting.Indented);
            return context.Response.WriteAsync(errorMessage);
        }
    }

    internal class ErrorDetails
    {
        public ErrorDetails(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public int StatusCode { get; }
        public string Message { get; }
    }
}