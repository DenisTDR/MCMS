using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Exceptions;

namespace MCMS.Data
{
    public static class RepositoryExtensions
    {
        #region Delete

        public static async Task<bool> SoftDelete<T>(this IRepository<T> repo, string id)
            where T : class, IEntity, ICanBeDeleted, new()
        {
            if (!await repo.Any(id))
            {
                throw new KnownException(code: 404);
            }

            return await repo.SoftDelete(repo.Attach(id));
        }

        public static async Task<bool> SoftDelete<T>(this IRepository<T> repo, T e)
            where T : class, IEntity, ICanBeDeleted
        {
            e.Deleted = true;
            await repo.SaveChangesIfNeeded();
            return true;
        }

        public static Task<int> SoftDelete<T>(this IRepository<T> repo, IEnumerable<string> ids)
            where T : class, IEntity, ICanBeDeleted, new()
        {
            return repo.SoftDelete(e => ids.Contains(e.Id));
        }

        public static async Task<int> SoftDelete<T>(this IRepository<T> repo, Expression<Func<T, bool>> predicate)
            where T : class, IEntity, ICanBeDeleted, new()
        {
            return await repo.DbSet.Where(predicate).UpdateFromQueryAsync(e => new T { Deleted = true });
        }

        #endregion
    }
}