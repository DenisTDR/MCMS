using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Exceptions;
using MCMS.Base.Helpers;
using MCMS.Display.TableConfig;
using MCMS.Models.Dt;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Data
{
    public class DtQueryService<TVm> where TVm : IViewModel
    {
        private readonly IMapper _mapper;
        private readonly ITableConfigServiceT<TVm> _tableConfigService;

        public DtQueryService(
            IMapper mapper,
            ITableConfigServiceT<TVm> tableConfigService
        )
        {
            _mapper = mapper;
            _tableConfigService = tableConfigService;
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
                var globalFilters = BuildFilters(globalSearchCols);
                if (globalFilters.Any())
                {
                    var qStr = string.Join(" || ", globalFilters.Select(gf => gf.Item1));
                    query = query.WhereDynamic(x => qStr, globalFilters.First().Item2);
                    isFiltered = true;
                }
            }

            var filters = BuildFilters(cols);

            foreach (var (qStr, qParams) in filters)
            {
                query = query.WhereDynamic(x => qStr, qParams);
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
                        var (qStr, qParameter) = DtQueryHelper.BuildCondition(col, dtColumn.Search.GetValueDecimal(),
                            objectName: "(string)(object)x", format: dtColumn.MatchedTableColumn.DbFuncFormat);
                        filters.Add((qStr, qParameter));
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
                        var terms = dtColumn.Search.Value.Split(" ",
                            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        var queryStrL = new List<string>();

                        if (terms.Length == 0)
                        {
                            break;
                        }

                        if (terms.Length > 7)
                        {
                            terms = new[] {dtColumn.Search.Value.Trim()};
                        }

                        for (var index = 0; index < terms.Length; index++)
                        {
                            var (qStr, qParameter) =
                                DtQueryHelper.BuildCondition(col, terms[index],
                                    dtColumn.MatchedTableColumn.DbFuncFormat, paramName: "param" + index);
                            queryStrL.Add(qStr);
                        }

                        var qStrFinal = string.Join(" && ", queryStrL);
                        var qParam = DummyDynamicQueryParams.Create(terms.Cast<object>().ToList());
                        filters.Add((qStrFinal, qParam));

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
                query = query.OrderBy(e => e.Updated);
            }
            else
            {
                foreach (var dtOrder in parameters.Order)
                {
                    var tc = parameters.Columns[dtOrder.Column].MatchedTableColumn;
                    var orderCol = tc.DbColumn;
                    var qStr = "x." + orderCol;
                    if (!string.IsNullOrEmpty(tc.DbFuncFormat))
                    {
                        qStr = string.Format(tc.DbFuncFormat, qStr);
                    }

                    if (dtOrder.Dir == DtOrderDir.Asc)
                    {
                        query = query.OrderByDynamic(x => qStr);
                    }
                    else
                    {
                        query = query.OrderByDescendingDynamic(x => qStr);
                    }
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