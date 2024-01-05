using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.MiddleWare
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using ValidLoaderShared.Utilities.Logging; // Assuming this is your custom logging utility

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISimplifiedLogger _logger; // Use your custom logger

        public ExceptionHandlingMiddleware(RequestDelegate next, ISimplifiedLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Log($"Unhandled exception: {ex.Message}", LogLevel.Error);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = $"An internal server error occurred. Please try again later. {exception.Message}",//TODO: to be conditioned by DEV enrionment only
            }.ToString());
        }
    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

}
