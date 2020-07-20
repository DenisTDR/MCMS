using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Base.Helpers
{
    public class EntityHelper
    {
        public static string GetEntityName<T>() where T : IEntity
        {
            return TypeHelpers.GetDisplayName(typeof(T));
        }



        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> UpdatablePrimitiveProperties =
            new ConcurrentDictionary<Type, List<PropertyInfo>>();

        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> UpdatableEntityProperties =
            new ConcurrentDictionary<Type, List<PropertyInfo>>();


        public static List<PropertyInfo> GetPrimitiveProperties<T>(IEnumerable<string> withNames = null)
            where T : IEntity
        {
            if (!UpdatablePrimitiveProperties.ContainsKey(typeof(T)))
            {
                UpdatablePrimitiveProperties[typeof(T)] = GetProps<T>()
                    .Where(prop => prop.PropertyType.IsPrimitive() || prop.PropertyType.IsEnum)
                    .ToList();
            }

            var result = UpdatablePrimitiveProperties[typeof(T)];
            if (withNames != null)
            {
                result = result.Where(p => withNames.Contains(p.Name.ToLower())).ToList();
            }

            return result;
        }


        public static List<PropertyInfo> GetEntityProperties<T>(IEnumerable<string> withNames = null)
            where T : IEntity
        {
            if (!UpdatableEntityProperties.ContainsKey(typeof(T)))
            {
                UpdatableEntityProperties[typeof(T)] =
                    GetProps<T>()
                        .Where(p => p.PropertyType.IsEntity())
                        .ToList();
            }

            var updatableProps = UpdatableEntityProperties[typeof(T)];
            if (withNames != null)
            {
                updatableProps = updatableProps.Where(p => withNames.Contains(p.Name.ToLower())).ToList();
            }

            return updatableProps.ToList();
        }


        private static IEnumerable<PropertyInfo> GetProps<T>()
        {
            return typeof(T).GetProperties()
                .Where(prop => !prop.GetCustomAttributes<NotMappedAttribute>().Any());
        }

        public static IEnumerable<string> GetPropertiesNames(Dictionary<string, bool> properties)
        {
            return properties.Where(kvp => kvp.Value).Select(kvp => kvp.Key.ToLower());
        }

        public static List<PropertyInfo> GetKeys<T>(DbContext dbContext) where T : IEntity, new()
        {
            return GetKeys(dbContext, typeof(T));
        }

        public static List<PropertyInfo> GetKeys(DbContext dbContext, Type eType)
        {
            if (eType == null)
            {
                throw new ArgumentNullException(nameof(eType));
            }

            var e = Activator.CreateInstance(eType);
            if (e == null)
            {
                throw new Exception($"Can't create instance of type '{eType.FullName}'");
            }

            var entry = dbContext.Entry(e);
            var primaryKey = entry.Metadata.FindPrimaryKey();
            return primaryKey.Properties.Select(p => p.PropertyInfo).ToList();
        }
    }
}