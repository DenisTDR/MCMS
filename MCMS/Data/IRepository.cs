using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace MCMS.Data
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T> GetOne(string id);
        Task<T> GetOne(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAll(bool dontFetch = false);
        Task<T> Add(T e);
        Task<T> Patch(string id, JsonPatchDocument<T> eub);

        Task<bool> Delete(string id);
        Task<bool> Delete(T e);

        Task<bool> Any(string id);
        Task<bool> Any(Expression<Func<T, bool>> predicate);

        void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func);
        void ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func);
        T Attach(T e);
        T Attach(string id);
        Task SaveChanges();
        bool SkipSaving { get; set; }
    }
}