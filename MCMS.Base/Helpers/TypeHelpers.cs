using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MCMS.Base.Extensions;

namespace MCMS.Base.Helpers
{
    public static class TypeHelpers
    {
        public static string GetDisplayNameOrDefault<T>()
        {
            return GetDisplayNameOrDefault(typeof(T));
        }

        public static string GetDisplayNameOrDefault(Type type)
        {
            return GetDisplayNameOrDefault(type as MemberInfo);
        }

        public static string GetDisplayNameOrDefault(PropertyInfo propertyInfo)
        {
            return GetDisplayNameOrDefault(propertyInfo as MemberInfo);
        }


        public static string GetDisplayName(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes<DisplayAttribute>().Select(a => a.Name)
                .FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? memberInfo
                .GetCustomAttributes<DisplayNameAttribute>().Select(a => a.DisplayName)
                .FirstOrDefault(n => !string.IsNullOrEmpty(n));
        }


        public static string GetDisplayNameOrDefault(MemberInfo memberInfo)
        {
            return GetDisplayName(memberInfo) ?? memberInfo.Name.ToSpacedPascalCase();
        }

        public static string GetDescription(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes<DisplayAttribute>().Select(a => a.Description)
                .FirstOrDefault(n => !string.IsNullOrEmpty(n));
        }


        public static string GetControllerName(Type type)
        {
            return type.Name.Replace("Controller", "");
        }
    }
}