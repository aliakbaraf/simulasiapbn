/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SimulasiAPBN.Web.Common.Middlewares
{
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
            
        public HttpLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<HttpLoggingMiddleware>();
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation($"HTTP Request Information:{Environment.NewLine}" + 
                                   $"Schema:{context.Request.Scheme} " +
                                   $"Host:{context.Request.Host} " +
                                   $"Method:{context.Request.Method} " +
                                   $"Path:{context.Request.Path} " +
                                   $"Query-String:{context.Request.QueryString}");
            
            await _next(context);
        }
    }
}