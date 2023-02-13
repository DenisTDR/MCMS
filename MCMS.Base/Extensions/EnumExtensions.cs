using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MCMS.Base.SwaggerFormly;

namespace MCMS.Base.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayDescription(this Enum value)
        {
            return value.GetDisplayProperty(attribute => attribute.Description);
        }

        public static string GetDisplayName(this Enum value)
        {
            return value.GetDisplayProperty(attribute => attribute.Name) ?? value?.ToString();
        }

        private static string GetDisplayProperty(this Enum value, Func<DisplayAttribute, string> func)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null) return null;
            var field = type.GetField(name);
            if (field == null) return null;
            var attr = field.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
            return attr != null ? func(attr) : null;
        }

        public static object GetCustomValue(this Enum value)
        {
            var enumType = value.GetType();
            var memberInfos = enumType.GetMember(value.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            object enumValue = enumValueMemberInfo?.GetCustomAttributes<EnumMemberAttribute>().FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty((string) enumValue))
            {
                enumValue = enumValueMemberInfo?.GetCustomAttributes<CustomEnumValueAttribute>().FirstOrDefault()
                    ?.Value;
            }

            return enumValue ?? value.ToString().ToCamelCase();
        }
    }
}