using System;
using System.Reflection;
using MCMS.Base.Extensions;

namespace MCMS.Base.Display.DisplayValue
{
    public static class DefaultDisplayValueFormatters
    {
        public static void RegisterFormatters(DisplayValueFormatters formatters)
        {
            formatters.Add(DisplayEnumValueName);
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
    }
}