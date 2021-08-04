using Colten.BAL.Interface;
using Colten.Common;
using Colten.Logging.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colten.API.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        private readonly AppSettings _appSettings;
        private readonly IEmailRepository _emailRepository;
        public ExceptionMiddleware(RequestDelegate next,
            ILoggerManager logger,
            IOptions<AppSettings> appSettings,
            IEmailRepository emailRepository)
        {
            _logger = logger;
            _next = next;
            _appSettings = appSettings.Value;
            _emailRepository = emailRepository;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (_appSettings.IsLog)
                {
                    _logger.LogError(ex.Message, ex);
                }
                if (_appSettings.IsEmail)
                {
                    await _emailRepository.SendExceptionEmail(httpContext.Request.Path.Value, ex);
                }
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new APIResponse()
            {
                ResponseStatus = ResponseStatus.FAILURE,
                ResponseMessage = Constant.ErrorMessage
            }.AsString());
        }

    }
}
