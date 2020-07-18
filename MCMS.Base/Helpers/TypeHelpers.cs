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
        public static string GetDisplayName<T>()
        {
            return GetDisplayName(typeof(T));
        }

        public static string GetDisplayName(Type type)
        {
            return GetDisplayName(type as MemberInfo);
        }

        public static string GetDisplayName(PropertyInfo propertyInfo)
        {
            return GetDisplayName(propertyInfo as MemberInfo);
        }

        public static string GetDisplayName(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes<DisplayAttribute>().Select(a => a.Name)
                .FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? memberInfo
                .GetCustomAttributes<DisplayNameAttribute>().Select(a => a.DisplayName)
                .FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? memberInfo.Name.ToSpacedPascalCase();
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