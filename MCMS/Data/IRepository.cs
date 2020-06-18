using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace MCMS.Data
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> GetOne(string id);
        Task<IList<T>> GetAll(bool dontFetch = false);
        Task<T> Add(T e);
        Task<T> Patch(string id, JsonPatchDocument<T> eub);

        Task<bool> Delete(string id);
        Task<bool> Delete(T e);

        void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func);
        void ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func);
    }
}