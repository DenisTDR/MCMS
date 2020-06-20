using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Data
{
    public class Repository<T> : IRepository<T> where T : Entity, new()
    {
        private readonly BaseDbContext _dbContext;
        private DbSet<T> _dbSet;
        private IQueryable<T> _queryable;

        public Repository(BaseDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
            _queryable = _dbSet;
        }

        public virtual Task<T> GetOne(string id)
        {
            return _queryable.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual Task<T> GetOne(Expression<Func<T, bool>> predicate)
        {
            return _queryable.FirstOrDefaultAsync(predicate);
        }

        public virtual Task<List<T>> GetAll(bool dontFetch = false)
        {
            return _queryable.ToListAsync();
        }

        public virtual async Task<T> Add(T e)
        {
            e.Id = null;
            var addingResult = await _dbSet.AddAsync(e);
            await _dbContext.SaveChangesAsync();
            return addingResult.Entity;
        }

        public virtual async Task<T> Patch(string id, JsonPatchDocument<T> patchDoc)
        {
            var e = await GetOne(id);
            if (patchDoc.IsEmpty())
            {
                return e;
            }

            patchDoc.ApplyTo(e);
            await _dbContext.SaveChangesAsync();
            return e;
        }

        public virtual async Task<bool> Delete(string id)
        {
            _dbSet.Remove(new T {Id = id});
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> Delete(T e)
        {
            _dbSet.Remove(e);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public virtual Task<bool> Any(string id)
        {
            return Any(e => e.Id == id);
        }

        public virtual Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return _queryable.AnyAsync(predicate);
        }

        public void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func)
        {
            _queryable = func(_dbSet);
        }

        public void ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func)
        {
            _queryable = func(_queryable);
        }
    }
}