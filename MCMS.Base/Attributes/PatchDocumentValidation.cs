using System;
using System.Linq;
using System.Reflection;
using MCMS.Base.Extensions;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCMS.Base.Attributes
{
    public class PatchDocumentValidation : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            foreach (var kvp in context.ActionArguments)
            {
                if (kvp.Value is IJsonPatchDocument doc &&
                    doc.GetType().IsSubclassOfGenericType(typeof(JsonPatchDocument<>)))
                {
                    if (!ValidateJsonPatchDocument(doc, context))
                    {
                        context.Result = new BadRequestObjectResult(context.ModelState);
                        return;
                    }
                }
            }
        }

        private bool ValidateJsonPatchDocument(IJsonPatchDocument doc, ActionExecutingContext context)
        {
            // doc.
            doc.GetType()
                .TryGetGenericTypeOfImplementedGenericType(typeof(JsonPatchDocument<>), out var buildGenericType);
            var argType = buildGenericType.GenericTypeArguments[0];
            if (!typeof(IFormModel).IsAssignableFrom(argType))
            {
                return true;
            }

            var mi = GetType().GetMethod(nameof(ValidateJsonPatchDocumentOfT),
                BindingFlags.NonPublic | BindingFlags.Instance);
            var gmi = mi?.MakeGenericMethod(argType);
            var resp = gmi?.Invoke(this, new object[] {doc, context}) as bool? == true;

            return resp;
        }

        private bool ValidateJsonPatchDocumentOfT<TFm>(JsonPatchDocument<TFm> doc, ActionExecutingContext context)
            where TFm : class, IFormModel, new()
        {
            var nfm = new TFm();
            foreach (var op in doc.Operations)
            {
                var splitPath = op?.path?.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (splitPath?.Length == 1 || splitPath?.Length == null)
                {
                    continue;
                }

                object obj = nfm;
                foreach (var pathPart in splitPath.Take(splitPath.Length - 1))
                {
                    obj = EnsureSubPropertyExists(obj, pathPart);
                }
            }

            doc.ApplyTo(nfm, context.ModelState);
            if (context.Controller is Controller controller)
            {
                controller.TryValidateModel(nfm);
            }

            var keys = context.ModelState.Keys.ToList();
            var updatedPaths = doc.Operations.Select(op => string.Join('.',
                    op.path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                        .Select(pathPath => pathPath.ToPascalCase())))
                .ToList();
            var diff = keys.Except(updatedPaths).ToList();
            diff.ForEach(d => context.ModelState.Remove(d));

            return context.ModelState.IsValid;
        }

        private object EnsureSubPropertyExists(object mainObj, string propertyName)
        {
            var propInfo = mainObj.GetType().GetProperty(propertyName.ToPascalCase()) ??
                           throw new Exception("Invalid property: " + propertyName);
            var subOjb = Activator.CreateInstance(propInfo.PropertyType);
            propInfo.SetValue(mainObj, subOjb);
            return subOjb;
        }
    }
}