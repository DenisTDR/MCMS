using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.Entities;

namespace MCMS.Helpers
{
    public class EntityHelper
    {
        public static string GetEntityName<T>() where T : IEntity
        {
            return TypeHelpers.GetDisplayName(typeof(T));
        }

        public static string GetPropertyName(PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes<DisplayAttribute>().ToList();
            if (attrs.Any())
            {
                var firstName = attrs.Select(a => a.Name).FirstOrDefault(name => !string.IsNullOrEmpty(name));
                if (!string.IsNullOrEmpty(firstName))
                {
                    return firstName;
                }
            }

            return propertyInfo.Name;
        }
    }
}