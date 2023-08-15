using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Extensions;

namespace MCMS.Base.Display.DisplayValue
{
    public static class DefaultDisplayValueFormatters
    {
        public static void RegisterFormatters(DisplayValueFormatters formatters)
        {
            formatters.Add(DisplayEnumValueName);
            formatters.Add(DisplayBooleanValue);
            formatters.Add(DisplayListValue);
        }

        public static bool DisplayEnumValueName(PropertyInfo pInfo, object o, out object value)
        {
            if (!pInfo.PropertyType.IsEnum)
            {
                value = null;
                return false;
            }

            var enumValue = pInfo.GetValue(o) as Enum;
            value = enumValue.GetDisplayName();

            return true;
        }

        public static bool DisplayBooleanValue(PropertyInfo pInfo, object o, out object value)
        {
            if (pInfo.PropertyType == typeof(bool) || pInfo.PropertyType == typeof(bool?))
            {
                var val = pInfo.GetValue(o);
                if (val is bool b)
                {
                    value = b
                        ? "<i class=\"far fa-check-circle fa-lg text-success\">"
                        : "<i class=\"far fa-times-circle fa-lg text-danger st-text\"></i>";
                    return true;
                }

                if (val == null && pInfo.PropertyType == typeof(bool?))
                {
                    value = "<i class=\"far fa-question-circle fa-lg text-secondary\">";
                    return true;
                }
            }

            value = null;
            return false;
        }

        public static bool DisplayListValue(PropertyInfo pInfo, object o, out object value)
        {
            if (typeof(IList).IsAssignableFrom(pInfo.PropertyType) && pInfo.GetValue(o) is IList src)
            {
                value = string.Join(", ", src.ToListDynamic().Select(ToStringJsonConverter.GetToStringValue));
                return true;
            }

            value = null;
            return false;
        }
    }
}