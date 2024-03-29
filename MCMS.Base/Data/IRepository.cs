using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Base.Data
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T> GetOne(string id);
        Task<T> GetOneOrThrow(string id);
        Task<T> GetOne(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null);
        Task<T> Add(T e);
        Task<T> Patch(string id, JsonPatchDocument<T> eub);
        Task<T> Patch(string id, JsonPatchDocument<T> eub, IAdapterFactory adapterFactory);

        #region Delete

        Task<bool> Delete(string id);
        Task<bool> Delete(T e);

        Task<int> Delete(IEnumerable<string> ids);
        Task<int> Delete(Expression<Func<T, bool>> predicate);

        #endregion

        Task<bool> Any(string id);
        Task<bool> Any(Expression<Func<T, bool>> predicate);

        void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func = null);
        IRepository<T> ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func);
        T Attach(T e);
        T Attach(string id);
        Task Reload(T e);
        Task SaveChanges();
        Task SaveChangesIfNeeded();
        bool SkipSaving { get; set; }
        DbSet<T> DbSet { get; }
        IQueryable<T> Queryable { get; }
        IQueryable<T> Query { get; }
    }
}