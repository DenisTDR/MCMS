using System;
using System.Linq;
using MCMS.Base.Data.Entities;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.JsonPatch;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Base.Data
{
    public static class ForeignEntityPatchHelper
    {
        public static void PatchEntityProperties<T>(T e, DbContext dbContext, JsonPatchDocument<T> patchDocument)
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

                object subE;

                var trackedEntityEntry = dbContext.ChangeTracker.Entries()
                    .Where(te => te.IsKeySet && eProp.PropertyType.IsInstanceOfType(te.Entity))
                    .FirstOrDefault(te => eKeys[0].GetValue(te.Entity)?.Equals(opBag.op.value) == true);

                if (trackedEntityEntry != null)
                {
                    subE = trackedEntityEntry.Entity;
                }
                else
                {
                    subE = Activator.CreateInstance(eProp.PropertyType) ?? throw new Exception(
                        $"Couldn't instantiate a '{eProp.PropertyType.Name}'.");
                    eKeys[0].SetValue(subE, opBag.op.value);
                    dbContext.Attach(subE);
                }

                eProp.SetValue(e, subE);
            }
        }
    }
}