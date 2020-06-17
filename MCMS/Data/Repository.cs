using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

#pragma warning disable 1998

namespace MCMS.Data
{
    public class Repository<T> : IRepository<T> where T : Entity
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

        public Task<T> GetOne(string id)
        {
            return _queryable.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IList<T>> GetAll(bool dontFetch = false)
        {
            return await _queryable.ToListAsync();
        }

        public async Task<T> Add(T e)
        {
            var addingResult = await _dbSet.AddAsync(e);
            await _dbContext.SaveChangesAsync();
            return addingResult.Entity;
        }

        public async Task<T> Patch(string id, JsonPatchDocument<T> eub)
        {
            var e = await GetOne(id);
            eub.ApplyTo(e);
            await _dbContext.SaveChangesAsync();
            return e;
        }

        public async Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
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