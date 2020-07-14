using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Data
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T> GetOne(string id);
        Task<T> GetOneOrThrow(string id);
        Task<T> GetOne(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAll(bool dontFetch = false);
        Task<T> Add(T e);
        Task<T> Patch(string id, JsonPatchDocument<T> eub);
        Task<T> Patch(string id, JsonPatchDocument<T> eub, IAdapterFactory adapterFactory);

        Task<bool> Delete(string id);
        Task<bool> Delete(T e);
        Task<bool> Delete(Expression<Func<T, bool>> predicate);

        Task<bool> Any(string id);
        Task<bool> Any(Expression<Func<T, bool>> predicate);

        void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func);
        void ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func);
        T Attach(T e);
        T Attach(string id);
        Task SaveChanges();
        bool SkipSaving { get; set; }
        DbSet<T> DbSet { get; }
        IQueryable<T> Queryable { get; }
    }
}