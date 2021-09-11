using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Exceptions;
using MCMS.Display.TableConfig;
using MCMS.Models.Dt;
using Microsoft.Extensions.Logging;

namespace MCMS.Data
{
    public class DtQueryService<TVm> : BaseDtQueryService<TVm> where TVm : IViewModel
    {
        private readonly List<TableColumn> _tableColumns;
        private readonly Dictionary<string, TableColumn> _matchedColumns = new();
        private readonly Dictionary<string, DbColumnMetadata> _dbColumnMetadataCache = new();

        public DtQueryService(IMapper mapper, ITableConfigServiceT<TVm> tableConfigService,
            ILoggerFactory loggerFactory) : base(mapper, loggerFactory)
        {
            _tableColumns = tableConfigService.GetTableColumns();
        }

        protected override bool IsServerSideFilterable(DtColumn column)
        {
            if (!column.Searchable) return false;
            var colConfig = TableColumn(column.Data);
            return colConfig.Searchable.IsServer();
        }

        protected override bool IsGlobalServerSideFilterable(DtColumn column)
        {
            if (!column.Searchable) return false;
            var colConfig = TableColumn(column.Data);
            return colConfig.Searchable.IsServer() && colConfig.Type < TableColumnType.Bool;
        }

        protected override bool IsServerSideSortable(DtColumn column)
        {
            if (!column.Orderable) return false;
            var colConfig = TableColumn(column.Data);
            return colConfig.Orderable.IsServer();
        }

        protected override DbColumnMetadata GetDbColumn(DtColumn column)
        {
            if (_dbColumnMetadataCache.ContainsKey(column.Data))
            {
                return _dbColumnMetadataCache[column.Data];
            }

            var colConfig = TableColumn(column.Data);
            return _dbColumnMetadataCache[column.Data] = new DbColumnMetadata()
                { DbColumn = colConfig.DbColumn, DbFuncFormat = colConfig.DbFuncFormat };
        }

        protected override QueryCondition BuildFilter(DtColumn dtColumn)
        {
            var dbColumn = GetDbColumn(dtColumn);
            var tableColumn = TableColumn(dtColumn.Data);
            switch (tableColumn.Type)
            {
                case TableColumnType.Number:
                {
                    if (!dtColumn.Search.HasValidNumber())
                    {
                        break;
                    }

                    if (DtQueryHelper.BuildMultiTermQuery(dtColumn, out var res, dbColumn))
                    {
                        return res;
                    }

                    break;
                }
                case TableColumnType.Bool:
                case TableColumnType.NullableBool:
                {
                    var res =
                        DtQueryHelper.BuildCondition(dbColumn.DbColumn,
                            dtColumn.Search.GetValueBool(tableColumn.Type == TableColumnType.NullableBool),
                            dbColumn.DbFuncFormat, true);
                    return res;
                }
                case TableColumnType.Select:
                {
                    var res =
                        DtQueryHelper.BuildCondition(dbColumn.DbColumn, dtColumn.Search.GetValueInteger(),
                            dbColumn.DbFuncFormat, true);
                    return res;
                }
                default:
                {
                    if (DtQueryHelper.BuildMultiTermQuery(dtColumn, out var res, dbColumn))
                    {
                        return res;
                    }

                    break;
                }
            }

            return new QueryCondition();
        }

        private TableColumn TableColumn(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new KnownException("Invalid column '" + data + "'.");
            }

            if (_matchedColumns.ContainsKey(data))
            {
                return _matchedColumns[data];
            }

            _matchedColumns[data] = _tableColumns.FirstOrDefault(col => col.Key == data);

            if (_matchedColumns[data] == null)
            {
                throw new KnownException("Invalid column '" + data + "'.");
            }

            return _matchedColumns[data];
        }
    }
}