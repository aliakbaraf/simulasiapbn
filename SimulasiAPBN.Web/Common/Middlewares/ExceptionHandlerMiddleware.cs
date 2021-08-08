/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Sentry;
using SimulasiAPBN.Web.Common.Diagnostics;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Models.Engine;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace SimulasiAPBN.Web.Common.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHub _hub;

        public ExceptionHandlerMiddleware(
            RequestDelegate next, 
            IWebHostEnvironment env, 
            ILogger<ExceptionHandlerMiddleware> logger,
            IHub hub)
        {
            _next = next;
            _env = env;
            _logger = logger;
            _hub = hub;
        }

        private async Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
        {

            var originalPath = context.Request.Path;
            var exception = edi?.SourceException;
            if (exception is null)
            {
                return;
            }

            if (context.Response.HasStarted) {
                edi.Throw();
            }

            var errorCode = EngineErrorCode.GenericServerError;
            var errorMessage = exception.Message;
            string supportId = null;
            var statusCode = HttpStatusCode.InternalServerError;

            context.Request.Path = "/Error".ToLower();

            try
            {
                switch (exception)
                {
                    case ValidationException:
                        errorCode = EngineErrorCode.DataValidationFailed;
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case GenericException genericException:
                        errorCode = genericException.Code;
                        statusCode = genericException.Status;
                        if (genericException.Code == EngineErrorCode.ApplicationNotInstalled ||
                            genericException.Code == EngineErrorCode.GenericClientError ||
                            genericException.Code == EngineErrorCode.GenericServerError ||
                            genericException.Code == EngineErrorCode.NoStateBudgetData)
                        {
                            supportId = _hub?.CaptureException(exception).ToString();
                        }
                        break;
                    case HttpRequestException:
                        if (exception.Message.StartsWith("Failed to proxy the request to http://localhost:"))
                        {
                            var serviceUnavailableExpcetion = new ServiceUnavailableException(
                                $"[Environment: {_env.EnvironmentName}] " +
                                $"The development server is currently down or being maintenance so this application " +
                                $"cannot be accessed at the moment.");
                            exception = serviceUnavailableExpcetion;
                            errorCode = serviceUnavailableExpcetion.Code;
                            errorMessage = serviceUnavailableExpcetion.Message;
                            statusCode = serviceUnavailableExpcetion.Status;
                        }
                        break;
                    default:
                        supportId = _hub?.CaptureException(exception).ToString();
                        break;
                }

                if (originalPath.StartsWithSegments("/Engine"))
                {
                    var error = new EngineError(errorCode, errorMessage, supportId);
                    var response = new EngineResponse(error);

                    context.Response.ContentType = "application/json; charset=utf-8";
                    context.Response.StatusCode = (int)statusCode;
                    await context.Response.WriteAsync(response.ToString());
                    return;
                }

                ClearHttpContext(context);

                var exceptionHandlerFeature = new ExceptionHandlerFeature(
                    originalPath,
                    exception,
                    errorCode,
                    statusCode,
                    supportId);
                context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);

                context.Response.StatusCode = (int) statusCode;
                context.Response.OnStarting(ClearCacheHeaders, context.Response);

                await _next(context);

                if (context.Response.StatusCode != (int) HttpStatusCode.NotFound)
                {
                    return;
                }

                _logger.LogError("Exception Handler Path is not found");
            }
            catch (Exception handlingException)
            {
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                _logger.LogError(handlingException.Message);
            }
            finally
            {
                context.Request.Path = originalPath;
            }

            edi.Throw();
        }

        public Task Invoke(HttpContext context)
        {
            ExceptionDispatchInfo edi;
            try
            {
                var task = _next(context);
                if (!task.IsCompletedSuccessfully)
                {
                    return Awaited(this, context, task);
                }

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
                edi = ExceptionDispatchInfo.Capture(exception);
            }

            return HandleException(context, edi);

            static async Task Awaited(ExceptionHandlerMiddleware middleware, HttpContext context, Task task)
            {
                ExceptionDispatchInfo edi = null;
                try
                {
                    await task;
                }
                catch (Exception exception)
                {
                    // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
                    edi = ExceptionDispatchInfo.Capture(exception);
                }

                if (edi != null)
                {
                    await middleware.HandleException(context, edi);
                }
            }
        }

        private static void ClearHttpContext(HttpContext context)
        {
            context.Response.Clear();

            // An endpoint may have already been set. Since we're going to re-invoke the middleware pipeline we need to reset
            // the endpoint and route values to ensure things are re-calculated.
            context.SetEndpoint(endpoint: null);
            var routeValuesFeature = context.Features.Get<IRouteValuesFeature>();
            routeValuesFeature?.RouteValues.Clear();
        }

        private static Task ClearCacheHeaders(object state)
        {
            var headers = ((HttpResponse)state).Headers;
            headers[HeaderNames.CacheControl] = "no-cache,no-store";
            headers[HeaderNames.Pragma] = "no-cache";
            headers[HeaderNames.Expires] = "-1";
            headers.Remove(HeaderNames.ETag);
            return Task.CompletedTask;
        }
    }

}
