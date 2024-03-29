using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
using MCMS.Base.JsonPatch;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            var resp = gmi?.Invoke(this, new object[] { doc, context }) as bool? == true;

            return resp;
        }

        private bool ValidateJsonPatchDocumentOfT<TFm>(JsonPatchDocument<TFm> doc, ActionExecutingContext context)
            where TFm : class, IFormModel, new()
        {
            var nfm = new TFm();
            for (var i = 0; i < doc.Operations.Count; i++)
            {
                var op = doc.Operations[i];
                try
                {
                    var splitPath = op?.path?.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (splitPath?.Length == null)
                    {
                        continue;
                    }

                    if (op.op != "remove" && IsSubEntityProperty(typeof(TFm), splitPath[0], out var propInfo))
                    {
                        var noPatchOnThisProp =
                            propInfo.GetCustomAttribute<DisablePatchSubPropertiesAttribute>() != null;
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

                    object obj = nfm;
                    if (splitPath.Length != 1)
                    {
                        // TODO: generalize this
                        if (obj.GetType().GetProperty(splitPath[0].ToPascalCase()) is { } prop &&
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

                        if (obj.GetType().ImplementsGenericInterface(typeof(IList<>)) &&
                            int.TryParse(splitPath[^1], out _))
                        {
                            EnsureSubPropertyExists(obj, splitPath[^1]);
                            continue;
                        }
                    }

                    var finalProp = obj.GetType().GetProperty(splitPath[^1].ToPascalCase());
                    if (finalProp?.CanWrite == false ||
                        finalProp?.GetCustomAttributes<FormlyFieldAttribute>().LastOrDefault()?.Disabled == true)
                    {
                        doc.Operations.Remove(op);
                        i--;
                    }
                }
                catch (KnownException exc)
                {
                    context.ModelState.AddModelError(op.path, exc.Message);
                    doc.Operations.Remove(op);
                    i--;
                }
            }

            var adapterFactory = context.HttpContext.RequestServices.Service<IAdapterFactory>() ??
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

            if (!context.ModelState.IsValid)
            {
                return false;
            }


            var legitValidationResults = new List<ValidationResult>();
            var items = new Dictionary<object, object>();

            var validationContext =
                new ValidationContext(nfm, context.HttpContext.RequestServices, items);
            var isValid = Validator.TryValidateObject(nfm, validationContext, legitValidationResults, true);

            if (isValid)
            {
                return true;
            }

            legitValidationResults =
                legitValidationResults.Where(vr => vr.MemberNames.Any(mn => updatedPaths.Contains(mn))).ToList();


            if (legitValidationResults.Any())
            {
                foreach (var vr in legitValidationResults)
                {
                    context.ModelState.AddModelError(string.Join(",", vr.MemberNames), vr.ErrorMessage);
                }
            }

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
                            list.Add(SafeCreateInstance(itemType));
                        }
                    }

                    return list[index];
                }
            }

            var propInfo = mainObj.GetType().GetProperty(propertyName.ToPascalCase()) ??
                           throw new KnownException("Invalid property: " + propertyName);
            if (propInfo.GetValue(mainObj) is { } existing)
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
                           throw new KnownException("Invalid property: " + propertyName);
            return typeof(IFormModel).IsAssignableFrom(propertyInfo.PropertyType) ||
                   typeof(IViewModel).IsAssignableFrom(propertyInfo.PropertyType);
        }

        private object SafeCreateInstance(Type type)
        {
            if (type == typeof(string))
            {
                return "";
            }

            return Activator.CreateInstance(type);
        }
    }
}