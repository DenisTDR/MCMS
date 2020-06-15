using System;
using System.Collections;
using System.Linq;
using MCMS.Base.Extensions;
using Microsoft.OpenApi.Any;
using MCMS.Exceptions;

namespace MCMS.SwaggerFormly.Extensions
{
    public static class OpenApiExtensions
    {
        public static IOpenApiAny ToOpenApi(object obj)
        {
            switch (obj)
            {
                case string s:
                    return new OpenApiString(s);
                case int i:
                    return new OpenApiInteger(i);
                case bool b:
                    return new OpenApiBoolean(b);
                case float f:
                    return new OpenApiFloat(f);
                case double d:
                    return new OpenApiDouble(d);
                case Enum e:
                    return ToOpenApi(e.GetCustomValue());
                case IEnumerable enumerable:
                    var arr = new OpenApiArray();
                    arr.AddRange(from object o in enumerable select ToOpenApi(o));
                    return arr;
                default:
                    throw new KnownException("Can't find any specific open api type for " + obj.GetType());
            }
        }
    }
}