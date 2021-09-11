using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Exceptions;
using MCMS.Models.Dt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MCMS.Data
{
    public abstract class BaseDtQueryService<TVm> where TVm : IViewModel
    {
        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        public bool AlreadyOrdered { get; set; }

        public BaseDtQueryService(
            IMapper mapper,
            ILoggerFactory loggerFactory
        )
        {
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger("DtQuery");
        }

        public Task<DtResult<TVm>> Query<TE>(IRepository<TE> repo, DtParameters parameters)
            where TE : class, IEntity
        {
            return Query(repo.Queryable, parameters);
        }

        public async Task<DtResult<TVm>> Query<TE>(IQueryable<TE> queryable, DtParameters parameters)
            where TE : class, IEntity
        {
            EnsureValidColumns(parameters);
            var result = new DtResult<TVm> { Draw = parameters.Draw };

            var query = queryable;
            result.RecordsTotal = await query.CountAsync();
            result.RecordsFiltered = result.RecordsTotal;

            query = ChainOrder(query, parameters);

            query = ChainFilter(query, parameters, out var isFiltered);
            if (isFiltered)
            {
                result.RecordsFiltered = await query.CountAsync();
            }

            query = ChainPagination(query, parameters);
            var entities = await query.ToListAsync();
            result.Data = _mapper.Map<List<TVm>>(entities);
            return result;
        }

        protected abstract bool IsServerSideFilterable(DtColumn column);
        protected abstract bool IsGlobalServerSideFilterable(DtColumn column);
        protected abstract bool IsServerSideSortable(DtColumn column);
        protected abstract DbColumnMetadata GetDbColumn(DtColumn column);

        protected abstract QueryCondition BuildFilter(DtColumn col);

        protected void EnsureValidColumns(DtParameters parameters)
        {
            foreach (var dtColumn in parameters.Columns.Where(dtc => dtc.Searchable))
            {
                if (dtColumn.Search.HasValue && !IsServerSideFilterable(dtColumn))
                {
                    throw new KnownException($"Invalid search operation on column '{dtColumn.Data}'");
                }
            }

            foreach (var dtOrder in parameters.Order)
            {
                if (dtOrder.Column >= parameters.Columns.Count ||
                    !IsServerSideSortable(parameters.Columns[dtOrder.Column]))
                {
                    throw new KnownException($"Invalid order operation on column '{dtOrder.Column}'");
                }
            }
        }

        private IQueryable<TE> ChainFilter<TE>(IQueryable<TE> query, DtParameters parameters, out bool isFiltered)
        {
            isFiltered = false;
            var cols = parameters.Columns.Where(IsServerSideFilterable).ToList();
            if (parameters.Search.HasValue)
            {
                var globalSearchCols = cols.Where(IsGlobalServerSideFilterable)
                    .Select(c => c.CloneForGlobalSearch(parameters.Search)).ToList();
                var gFilters = BuildFilters(globalSearchCols);
                if (gFilters.Any())
                {
                    var qStr = string.Join(" || ", gFilters.Select(gf => gf.Query));
                    try
                    {
                        query = query.WhereDynamic(x => qStr, gFilters.First().Params);
                        _logger.LogInformation("queryStr= {Query}\nparams={Params}", qStr,
                            JsonConvert.SerializeObject(gFilters.First().Params));
                    }
                    catch
                    {
                        _logger.LogError("queryStr: {Query}", qStr);
                        _logger.LogError("params: {Params}", JsonConvert.SerializeObject(gFilters.First().Params));
                        throw;
                    }

                    isFiltered = true;
                }
            }

            var filters = BuildFilters(cols.Where(col => col?.Search?.HasValue == true));

            foreach (var qCond in filters)
            {
                try
                {
                    query = query.WhereDynamic(x => qCond.Query, qCond.Params);
                    _logger.LogInformation("queryStr= {Query}\nparams={Params}", qCond.Query,
                        JsonConvert.SerializeObject(qCond.Params));
                }
                catch
                {
                    _logger.LogError("queryStr: {Query}", qCond.Query);
                    _logger.LogError("params: {Params}", JsonConvert.SerializeObject(qCond.Params));
                    throw;
                }
            }

            isFiltered = isFiltered || filters.Any();

            return query;
        }

        private List<QueryCondition> BuildFilters(IEnumerable<DtColumn> cols)
        {
            return cols.Select(BuildFilter).Where(result => result.Valid).ToList();
        }

        private IQueryable<TE> ChainOrder<TE>(IQueryable<TE> query, DtParameters parameters)
            where TE : class, IEntity
        {
            if (parameters.Order.Count == 0)
            {
                if (!AlreadyOrdered)
                {
                    query = query.OrderBy(e => e.Updated);
                }
            }
            else
            {
                foreach (var dtOrder in parameters.Order)
                {
                    var dbColumn = GetDbColumn(parameters.Columns[dtOrder.Column]);
                    var qStr = $"x.{dbColumn.DbColumn}";
                    if (!string.IsNullOrEmpty(dbColumn.DbFuncFormat))
                    {
                        qStr = string.Format(dbColumn.DbFuncFormat, qStr);
                    }

                    query = dtOrder.Dir == DtOrderDir.Asc
                        ? query.OrderByDynamic(x => qStr)
                        : query.OrderByDescendingDynamic(x => qStr);
                }
            }

            return query;
        }

        private IQueryable<TE> ChainPagination<TE>(IQueryable<TE> query, DtParameters parameters)
            where TE : class, IEntity
        {
            if (parameters.Start > 0)
            {
                query = query.Skip(parameters.Start);
            }

            if (parameters.Length > 0)
            {
                query = query.Take(parameters.Length);
            }

            return query;
        }
    }

    public struct DbColumnMetadata
    {
        public string DbColumn { get; set; }
        public string DbFuncFormat { get; set; }

        public DbColumnMetadata(string dbColumn, string dbFuncFormat)
        {
            DbColumn = dbColumn;
            DbFuncFormat = dbFuncFormat;
        }
    }
}