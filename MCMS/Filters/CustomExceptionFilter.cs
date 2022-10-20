using System;
using System.Linq;
using System.Reflection;
using MCMS.Base.Controllers.Api;
using MCMS.Base.Exceptions;
using MCMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MCMS.Filters
{
    internal class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (IsApiController(context))
            {
                var responseModel = new { Error = GetRelevantExceptionMessage(context.Exception) };
                var dr = new ObjectResult(responseModel);

                if (context.Exception is KnownException knownExc)
                {
                    dr.StatusCode = knownExc.Code != 0 ? knownExc.Code : 500;
                }
                else
                {
                    Console.Error.WriteLine("Uncaught Exception: " + context.Exception + " ");
                    dr.StatusCode = 500;
                }

                context.Result = dr;
            }
            else
            {
                if (context.Exception is KnownException knownExc && knownExc.Code != 0 && knownExc.Code != 500
                    && !context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                    {
                        Model = new ErrorViewModel
                        {
                            Exception = knownExc,
                            RequestId = context.HttpContext.TraceIdentifier,
                            StatusCode = knownExc.Code
                        }
                    };
                    var result = new ViewResult
                    {
                        ViewName = "Error",
                        ViewData = viewData,
                    };

                    context.ExceptionHandled = true;

                    context.Result = result;
                }
            }
        }

        private bool IsApiController(ExceptionContext context)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor actionDescriptor))
            {
                return false;
            }

            var pAttrs = actionDescriptor.MethodInfo.GetCustomAttributes<ProducesAttribute>().ToList();
            if (!pAttrs.Any())
            {
                pAttrs = actionDescriptor.ControllerTypeInfo.GetCustomAttributes<ProducesAttribute>().ToList();
            }

            if (!pAttrs.Any())
            {
                return typeof(BaseApiController).IsAssignableFrom(actionDescriptor.ControllerTypeInfo);
            }

            return pAttrs.Any(a => a.ContentTypes.Any(c => c.ToLower().StartsWith("application/json")));
        }

        private string GetRelevantExceptionMessage(Exception exc)
        {
            if (string.IsNullOrEmpty(exc?.Message))
            {
                return "Unknown error";
            }

            var msg = exc.Message;
            while (msg.Contains("See the inner exception for details") && exc.InnerException != null)
            {
                exc = exc.InnerException;
                msg = exc.Message;
            }

            return msg;
        }
    }
}