using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;
using MCMS.Models.Dt;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Data
{
    public class DtQueryService
    {
        private IMapper _mapper;

        public DtQueryService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<DtResult<T>> Query<T, TE>(IRepository<TE> repo, DtParameters parameters)
            where TE : class, IEntity
        {
            var result = new DtResult<T>();
            result.Draw = parameters.Draw;

            var query = repo.Queryable;
            result.RecordsTotal = await query.CountAsync();
            result.RecordsFiltered = result.RecordsTotal;
            var isFiltered = false;

            if (parameters.Order.Count == 0)
            {
                query = query.OrderBy(e => e.Updated);
            }
            else
            {
                foreach (var dtOrder in parameters.Order)
                {
                    var orderCol = parameters.Columns[dtOrder.Column].Data;
                    orderCol = orderCol.ToPascalCase();
                    if (dtOrder.Dir == DtOrderDir.Asc)
                    {
                        query = query.OrderByDynamic(x => "x." + orderCol);
                    }
                    else
                    {
                        query = query.OrderByDescendingDynamic(x => "x." + orderCol);
                    }
                }
            }

            foreach (var dtColumn in parameters.Columns)
            {
                if (!string.IsNullOrEmpty(dtColumn?.Search?.Value))
                {
                    var terms = dtColumn.Search.Value.Split(" ",
                        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    var col = dtColumn.Data.ToPascalCase();

                    foreach (var term in terms)
                    {
                        var filterQuery = "EF.Functions.ILike(x." + col + ", \"%" + term + "%\")";
                        query = query.WhereDynamic(x => filterQuery);
                        isFiltered = true;
                    }
                }
            }

            if (isFiltered)
            {
                result.RecordsFiltered = await query.CountAsync();
            }

            if (parameters.Start > 0)
            {
                query = query.Skip(parameters.Start);
            }

            if (parameters.Length > 0)
            {
                query = query.Take(parameters.Length);
            }


            result.Data = _mapper.Map<List<T>>(await query.ToListAsync());
            return result;
        }
    }
}