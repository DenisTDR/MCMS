using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.TableConfig
{
    public class TableConfigServiceOfT<TVm> : TableConfigService, ITableConfigServiceT<TVm> where TVm : IViewModel
    {
        public TableConfigServiceOfT(IUrlHelper urlHelper)
        {
            UrlHelper = urlHelper;
        }

        public virtual Type ViewModelType => typeof(TVm);
        public IUrlHelper UrlHelper { get; set; }
        public override string ModelName => TypeHelpers.GetDisplayNameOrDefault<TVm>();

        public override List<TableColumn> GetTableColumns()
        {
            var props = ViewModelType.GetProperties().ToList();
            var tableColumnProps = props.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<TableColumnAttribute>().FirstOrDefault();
                return attr != null && (!attr.Hidden || attr.RowGroup);
            }).ToList();
            if (tableColumnProps.Count == 0)
            {
                tableColumnProps = props;
            }

            tableColumnProps = tableColumnProps.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<TableColumnAttribute>().FirstOrDefault();
                return attr is not { Hidden: true } || attr.RowGroup;
            }).ToList();

            var list = tableColumnProps.Select(prop =>
                new TableColumn(prop, prop.GetCustomAttributes<TableColumnAttribute>().ToList())).ToList();

            return list;
        }
    }
}