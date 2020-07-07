using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public abstract class ModelDisplayConfigService: IModelDisplayConfigService
    {
        public abstract Type ViewModelType { get; }

        public abstract ModelDisplayTableConfig GetTableConfig(IUrlHelper url, dynamic viewBag, bool createNewLink = true);

        public virtual List<TableColumn> GetTableColumns(bool excludeActionsColumn = false)
        {
            var props = ViewModelType.GetProperties().ToList();
            var tableColumnProps = props.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<TableColumnAttribute>().FirstOrDefault();
                return attr != null && !attr.Hidden;
            }).ToList();
            if (tableColumnProps.Count == 0)
            {
                tableColumnProps = props;
            }

            tableColumnProps = tableColumnProps.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<TableColumnAttribute>().FirstOrDefault();
                return attr == null || !attr.Hidden;
            }).ToList();

            var list = tableColumnProps
                .Select(prop => new TableColumn(TypeHelpers.GetDisplayName(prop), prop.Name.ToCamelCase(),
                    prop.GetCustomAttributes<TableColumnAttribute>().FirstOrDefault()?.Order ?? 0)).ToList();
            if (!excludeActionsColumn)
            {
                list.Add(new TableColumn("Actions", "_actions", 100));
            }

            return list;
        }

        public virtual List<DetailsField> GetDetailsFields(bool excludeActionsColumn = false)
        {
            var props = ViewModelType.GetProperties().ToList();
            var detailsFields = props.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<DetailsFieldAttribute>().FirstOrDefault();
                return attr != null && !attr.Hidden;
            }).ToList();
            if (detailsFields.Count == 0)
            {
                detailsFields = props;
            }

            detailsFields = detailsFields.Where(prop =>
            {
                var attr = prop.GetCustomAttributes<DetailsFieldAttribute>().FirstOrDefault();
                return attr == null || !attr.Hidden;
            }).ToList();

            var list = detailsFields
                .Select(prop => new DetailsField(TypeHelpers.GetDisplayName(prop), prop,
                    prop.GetCustomAttributes<DetailsFieldAttribute>().FirstOrDefault()?.Order ?? 0)).ToList();

            return list;
        }
    }
}