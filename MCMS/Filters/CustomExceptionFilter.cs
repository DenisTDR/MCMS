using System;
using MCMS.Controllers.Api;
using MCMS.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCMS.Filters
{
    internal class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (IsApiController(context))
            {
                var responseModel = new {Error = context.Exception?.Message};
                var dr = new ObjectResult(responseModel);

                if (context.Exception is KnownException knownExc)
                {
                    dr.StatusCode = knownExc.Code != 0 ? knownExc.Code : 500;
                }
                else
                {
                    Console.Error.WriteLine("Uncatched Exception: " + context.Exception + " ");
                    dr.StatusCode = 500;
                }

                context.Result = dr;
            }
            else
            {
                if (context.Exception is KnownException knownExc && knownExc.Code != 0 && knownExc.Code != 500)
                {
                    var result = new StatusCodeResult(knownExc.Code);
                    context.Result = result;
                }
            }
        }

        private bool IsApiController(ExceptionContext context)
        {
            return context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                   typeof(ApiController).IsAssignableFrom(actionDescriptor.ControllerTypeInfo);
        }


        // context.HttpContext.Items["ForcedLayout"] = "_ModalLayout";
    }
}