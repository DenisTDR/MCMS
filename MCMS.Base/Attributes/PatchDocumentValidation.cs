using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.Base.JsonPatch;
using MCMS.Base.SwaggerFormly.Formly;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

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
            for (var i = 0; i < doc.Operations.Count; i++)
            {
                var op = doc.Operations[i];
                var splitPath = op?.path?.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (splitPath?.Length == null)
                {
                    continue;
                }

                if (op.op != "remove" && IsSubEntityProperty(typeof(TFm), splitPath[0], out var propInfo))
                {
                    var noPatchOnThisProp = propInfo.GetCustomAttribute<DisablePatchSubPropertiesAttribute>() != null;
                    if (op.value is JObject jObj)
                    {
                        doc.Operations.Remove(op);
                        i--;
                        foreach (var kvp in jObj)
                        {
                            if (noPatchOnThisProp && kvp.Key != "id")
                            {
                                continue;
                            }

                            doc.Operations.Add(new Operation<TFm>(op.op, op.path + "/" + kvp.Key, op.from,
                                kvp.Value.ToObject(propInfo.PropertyType.GetProperty(kvp.Key.ToPascalCase())
                                    ?.PropertyType)));
                        }
                    }
                }

                if (splitPath.Length == 1)
                {
                    continue;
                }

                object obj = nfm;
                
                // TODO: generalize this
                if (obj.GetType().GetProperty(splitPath[0].ToPascalCase()) is {} prop &&
                    splitPath[1].ToPascalCase() != "Id" &&
                    prop.GetCustomAttribute<DisablePatchSubPropertiesAttribute>() != null)
                {
                    doc.Operations.Remove(op);
                    i--;
                    continue;
                }

                foreach (var pathPart in splitPath.Take(splitPath.Length - 1))
                {
                    obj = EnsureSubPropertyExists(obj, pathPart);
                }

                if (obj.GetType().GetProperty(splitPath[^1].ToPascalCase())?.CanWrite == false)
                {
                    doc.Operations.Remove(op);
                    i--;
                }
            }

            var adapterFactory = context.HttpContext.RequestServices.GetService<IAdapterFactory>() ??
                                 new AdapterFactory();
            doc.ApplyTo(nfm, adapterFactory, context.ModelState);
            if (!context.ModelState.IsValid)
            {
                return false;
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
            if (mainObj.GetType().ImplementsGenericInterface(typeof(IList<>)))
            {
                if (int.TryParse(propertyName, out var index) && index.ToString() == propertyName)
                {
                    var list = mainObj as IList;
                    if (list.Count <= index)
                    {
                        var itemType = mainObj.GetType()
                            .GetGenericArgumentTypeOfImplementedGenericInterface(typeof(IList<>));
                        while (list.Count <= index)
                        {
                            list.Add(Activator.CreateInstance(itemType));
                        }
                    }

                    return list[index];
                }
            }

            var propInfo = mainObj.GetType().GetProperty(propertyName.ToPascalCase()) ??
                           throw new Exception("Invalid property: " + propertyName);
            if (propInfo.GetValue(mainObj) is {} existing)
            {
                return existing;
            }

            var subOjb = Activator.CreateInstance(propInfo.PropertyType);
            propInfo.SetValue(mainObj, subOjb);
            return subOjb;
        }

        private bool IsSubEntityProperty(Type objType, string propertyName, out PropertyInfo propertyInfo)
        {
            propertyInfo = objType.GetProperty(propertyName.ToPascalCase()) ??
                           throw new Exception("Invalid property: " + propertyName);
            return typeof(IFormModel).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(IViewModel).IsAssignableFrom(propertyInfo.PropertyType);
        }
    }
}