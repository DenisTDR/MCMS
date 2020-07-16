using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Exceptions;
using MCMS.Base.JsonPatch;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Data
{
    public class Repository<T> : IRepository<T> where T : class, IEntity, new()
    {
        protected readonly BaseDbContext DbContext;
        public DbSet<T> DbSet { get; }
        public IQueryable<T> Queryable { get; protected set; }
        public bool SkipSaving { get; set; }

        public Repository(BaseDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<T>();
            Queryable = DbSet;
            if (typeof(IOrderable).IsAssignableFrom(typeof(T)))
            {
                Queryable = Queryable.OrderBy(e => ((IOrderable) e).Order);
            }
        }

        public virtual Task<T> GetOne(string id)
        {
            var query = typeof(ISluggable).IsAssignableFrom(typeof(T))
                ? Queryable.Where(e => e.Id == id || ((ISluggable) e).Slug == id)
                : Queryable.Where(e => e.Id == id);
            return query.FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetOneOrThrow(string id)
        {
            var e = await GetOne(id);
            if (e == null)
            {
                throw new KnownException(code: 404);
            }

            return e;
        }

        public virtual Task<T> GetOne(Expression<Func<T, bool>> predicate)
        {
            return Queryable.FirstOrDefaultAsync(predicate);
        }

        public virtual Task<List<T>> GetAll()
        {
            return Queryable.ToListAsync();
        }

        public virtual async Task<T> Add(T e)
        {
            e.Id = null;
            var addingResult = await DbSet.AddAsync(e);
            await SaveChangesAsyncIfNeeded();
            return addingResult.Entity;
        }

        public virtual Task<T> Patch(string id, JsonPatchDocument<T> patchDoc)
        {
            return Patch(id, patchDoc, null);
        }

        public virtual async Task<T> Patch(string id, JsonPatchDocument<T> patchDoc, IAdapterFactory adapterFactory)
        {
            var e = await GetOne(id);
            if (patchDoc.IsEmpty())
            {
                return e;
            }

            ForeignEntityPatchHelper.PatchEntityProperties(e, DbContext, patchDoc);
            if (adapterFactory == null)
            {
                patchDoc.ApplyTo(e);
            }
            else
            {
                patchDoc.ApplyTo(e, adapterFactory);
            }

            await SaveChangesAsyncIfNeeded();
            return e;
        }

        public virtual async Task<bool> Delete(string id)
        {
            if (!await Any(id))
            {
                throw new KnownException(code: 404);
            }

            return await Delete(new T {Id = id});
        }

        public virtual async Task<bool> Delete(T e)
        {
            DbSet.Remove(e);
            await SaveChangesAsyncIfNeeded();
            return true;
        }

        public virtual async Task<bool> Delete(Expression<Func<T, bool>> predicate)
        {
            DbSet.RemoveRange(DbSet.Where(predicate));
            await SaveChangesAsyncIfNeeded();
            return true;
        }

        public virtual Task<bool> Any(string id)
        {
            return Any(e => e.Id == id);
        }

        public virtual Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return Queryable.AnyAsync(predicate);
        }

        public void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func)
        {
            Queryable = func(DbSet);
        }

        public void ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func)
        {
            Queryable = func(Queryable);
        }

        public T Attach(T e)
        {
            return DbContext.Attach(e).Entity;
        }

        public T Attach(string id)
        {
            return DbContext.Attach(new T {Id = id}).Entity;
        }

        public Task SaveChanges() => DbContext.SaveChangesAsync();
        protected Task SaveChangesAsyncIfNeeded() => !SkipSaving ? SaveChanges() : Task.CompletedTask;
    }
}