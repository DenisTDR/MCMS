using System;
using System.Linq;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;
using MCMS.Exceptions;
using MCMS.Helpers;
using Microsoft.AspNetCore.JsonPatch;

namespace MCMS.Data
{
    public static class ForeignEntityPatchHelper
    {
        public static void PatchEntityProperties<T>(T e, BaseDbContext dbContext, JsonPatchDocument<T> patchDocument)
            where T : class, IEntity
        {
            var eProps = EntityHelper.GetEntityProperties<T>();
            var opBags = patchDocument.Operations.Select(op => new {op, path = op.GetSplitPath()}).ToList();
            foreach (var opBag in opBags.Where(ob => ob.path.Count > 1))
            {
                var eProp = eProps.FirstOrDefault(ep => ep.Name.ToCamelCase() == opBag.path[0]);
                if (eProp == null)
                {
                    continue;
                }

                var eKeys = EntityHelper.GetKeys(dbContext, eProp.PropertyType);
                if (eKeys.Count != 1)
                {
                    throw new KnownException(
                        "Can't patch an linked entity with 0 or more than 1 properties in the primary key.");
                }

                var eKeyProp = eKeys[0].Name.ToCamelCase();
                if (eKeyProp != opBag.path[1])
                {
                    continue;
                }

                var subE = Activator.CreateInstance(eProp.PropertyType) ?? throw new Exception(
                    $"Couldn't instantiate a '{eProp.PropertyType.Name}'.");
                eKeys[0].SetValue(subE, opBag.op.value);
                dbContext.Attach(subE);
                eProp.SetValue(e, subE);
            }
        }
    }
}