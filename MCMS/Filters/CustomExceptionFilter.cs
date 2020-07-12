using System;
using MCMS.Base.Exceptions;
using MCMS.Controllers.Api;
using MCMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
                    var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                    {
                        Model = new ErrorViewModel
                        {
                            Exception = knownExc,
                            RequestId = context.HttpContext.TraceIdentifier,
                            StatusCode = knownExc.Code
                        }
                    };
                    var result = new ViewResult {
                        ViewName = "Error",
                        ViewData = viewData,
                    };

                    context.ExceptionHandled = true;
                    
                    // var result = new StatusCodeResult(knownExc.Code);
                    // var result = new ObjectResult(Error = context.Exception?.Message);
                    context.Result = result;
                }
            }
        }

        private bool IsApiController(ExceptionContext context)
        {
            return context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                   typeof(AdminApiController).IsAssignableFrom(actionDescriptor.ControllerTypeInfo);
        }


        // context.HttpContext.Items["ForcedLayout"] = "_ModalLayout";
    }
}