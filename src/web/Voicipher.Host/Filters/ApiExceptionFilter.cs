using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Voicipher.Business.Utils;
using Voicipher.Domain.Exceptions;

namespace Voicipher.Host.Filters
{
    public sealed class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ApiExceptionFilter(ILogger logger)
        {
            _logger = logger.ForContext<ApiExceptionFilter>();
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationErrorException ex)
            {
                context.HttpContext.Response.StatusCode = ex.HttpStatusCode;
                context.Result = new JsonResult(ex.ErrorCode);
            }
            else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = new JsonResult(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Operation failed"
                });

                _logger.Error($"{ExceptionFormatter.FormatException(context.Exception)}");
            }

            base.OnException(context);
        }
    }
}
