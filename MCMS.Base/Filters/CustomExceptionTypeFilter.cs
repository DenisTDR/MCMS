using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCMS.Base.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public Type ExceptionType { get; }
        private readonly string _message;
        private readonly int _statusCode;

        public CustomExceptionFilterAttribute(Type exceptionType, string message, int statusCode = 400)
        {
            _message = message;
            _statusCode = statusCode;
            ExceptionType = exceptionType;
        }

        public override void OnException(ExceptionContext context)
        {
            if (ExceptionType.IsInstanceOfType(context.Exception))
            {
                var responseModel = new { Error = _message };
                var dr = new ObjectResult(responseModel)
                {
                    StatusCode = _statusCode
                };

                context.ExceptionHandled = true;
                context.Result = dr;
                return;
            }

            base.OnException(context);
        }
    }
}