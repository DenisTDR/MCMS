using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Exceptions;
using MCMS.Display.TableConfig;
using MCMS.Models.Dt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MCMS.Data
{
    public class DtQueryService<TVm> where TVm : IViewModel
    {
        private readonly IMapper _mapper;
        private readonly ITableConfigServiceT<TVm> _tableConfigService;
        private readonly ILogger _logger;

        public bool AlreadyOrdered { get; set; }

        public DtQueryService(
            IMapper mapper,
            ITableConfigServiceT<TVm> tableConfigService,
            ILoggerFactory loggerFactory
        )
        {
            _mapper = mapper;
            _tableConfigService = tableConfigService;
            _logger = loggerFactory.CreateLogger("DtQuery");
        }

        public async Task<DtResult<TVm>> Query<TE>(IRepository<TE> repo, DtParameters parameters)
            where TE : class, IEntity
        {
            EnsureValidColumns(parameters);
            var result = new DtResult<TVm> {Draw = parameters.Draw};

            var query = repo.Queryable;
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

        private void EnsureValidColumns(DtParameters parameters)
        {
            var tableCols = _tableConfigService.GetTableColumns();
            foreach (var dtColumn in parameters.Columns.Where(dtc => dtc.Searchable || dtc.Orderable))
            {
                dtColumn.MatchedTableColumn = tableCols.FirstOrDefault(tc => tc.Key == dtColumn.Data);
                if (dtColumn.MatchedTableColumn == null ||
                    dtColumn.Search.HasValue && !dtColumn.MatchedTableColumn.Searchable.IsServer())
                {
                    throw new KnownException("Invalid operation on column '" + dtColumn.Data + "'");
                }
            }

            foreach (var dtOrder in parameters.Order)
            {
                if (dtOrder.Column >= parameters.Columns.Count ||
                    parameters.Columns[dtOrder.Column].MatchedTableColumn == null ||
                    !parameters.Columns[dtOrder.Column].MatchedTableColumn.Orderable.IsServer())
                {
                    throw new KnownException("Invalid order operation on column '" + dtOrder.Column + "'");
                }
            }
        }

        private IQueryable<TE> ChainFilter<TE>(IQueryable<TE> query, DtParameters parameters, out bool isFiltered)
        {
            isFiltered = false;
            var cols = parameters.Columns.Where(c => c.Searchable && c.MatchedTableColumn.Searchable.IsServer())
                .ToList();
            if (parameters.Search.HasValue)
            {
                var globalSearchCols = cols.Where(c => c.MatchedTableColumn.Type < TableColumnType.Bool)
                    .Select(c => c.CloneForGlobalSearch(parameters.Search)).ToList();
                var gFilters = BuildFilters(globalSearchCols);
                if (gFilters.Any())
                {
                    var qStr = string.Join(" || ", gFilters.Select(gf => gf.Item1));
                    try
                    {
                        query = query.WhereDynamic(x => qStr, gFilters.First().Item2);
                        _logger.LogInformation("queryStr= {Query}\nparams={Params}", qStr,
                            JsonConvert.SerializeObject(gFilters.First().Item2));
                    }
                    catch
                    {
                        _logger.LogError("queryStr: {Query}", qStr);
                        _logger.LogError("params: {Params}", JsonConvert.SerializeObject(gFilters.First().Item2));
                        throw;
                    }

                    isFiltered = true;
                }
            }

            var filters = BuildFilters(cols);

            foreach (var (qStr, qParams) in filters)
            {
                try
                {
                    query = query.WhereDynamic(x => qStr, qParams);
                    _logger.LogInformation("queryStr= {Query}\nparams={Params}", qStr,
                        JsonConvert.SerializeObject(qParams));
                }
                catch
                {
                    _logger.LogError("queryStr: {Query}", qStr);
                    _logger.LogError("params: {Params}", JsonConvert.SerializeObject(qParams));
                    throw;
                }
            }

            isFiltered = isFiltered || filters.Any();

            return query;
        }

        private List<(string, object)> BuildFilters(IEnumerable<DtColumn> cols)
        {
            var filters = new List<(string, object)>();

            foreach (var dtColumn in cols.Where(c => c?.Search?.HasValue == true))
            {
                var col = dtColumn.MatchedTableColumn.DbColumn;
                switch (dtColumn.MatchedTableColumn.Type)
                {
                    case TableColumnType.Number:
                    {
                        if (!dtColumn.Search.HasValidNumber())
                        {
                            break;
                        }

                        if (DtQueryHelper.BuildMultiTermQuery(dtColumn, out var res, true))
                        {
                            filters.Add((res.quertStr, res.parameters));
                        }

                        break;
                    }
                    case TableColumnType.Bool:
                    case TableColumnType.NullableBool:
                    {
                        var (qStr, qParameter) =
                            DtQueryHelper.BuildCondition(col,
                                dtColumn.Search.GetValueBool(
                                    dtColumn.MatchedTableColumn.Type == TableColumnType.NullableBool),
                                dtColumn.MatchedTableColumn.DbFuncFormat, true);
                        filters.Add((qStr, qParameter));
                        break;
                    }
                    case TableColumnType.Select:
                    {
                        var (qStr, qParameter) =
                            DtQueryHelper.BuildCondition(col, dtColumn.Search.GetValueInteger(),
                                dtColumn.MatchedTableColumn.DbFuncFormat, true);
                        filters.Add((qStr, qParameter));
                        break;
                    }
                    default:
                    {
                        if (DtQueryHelper.BuildMultiTermQuery(dtColumn, out var res))
                        {
                            filters.Add((res.quertStr, res.parameters));
                        }

                        break;
                    }
                }
            }

            return filters;
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
                    var tc = parameters.Columns[dtOrder.Column].MatchedTableColumn;
                    var orderCol = tc.DbColumn;
                    var qStr = $"x.{orderCol}";
                    if (!string.IsNullOrEmpty(tc.DbFuncFormat))
                    {
                        qStr = string.Format(tc.DbFuncFormat, qStr);
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
}