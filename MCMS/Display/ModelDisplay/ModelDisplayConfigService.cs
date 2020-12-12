using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public abstract class ModelDisplayConfigService : IModelDisplayConfigService
    {
        protected ITranslationsRepository TranslationsRepository { get; }
        public abstract Type ViewModelType { get; }

        public abstract Task<IndexPageConfig> GetIndexPageConfig(IUrlHelper url, dynamic viewBag,
            bool createNewLink = true);

        public abstract Task<TableDisplayConfig> GetTableConfig(IUrlHelper url, dynamic viewBag,
            bool createNewLink = true);


        public ModelDisplayConfigService(ITranslationsRepository translationsRepository)
        {
            TranslationsRepository = translationsRepository;
        }

        public virtual async Task<List<TableColumn>> GetTableColumns(bool excludeActionsColumn = false)
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
                return attr == null || !attr.Hidden || attr.RowGroup;
            }).ToList();

            var list = tableColumnProps.Select(prop => new TableColumn(TypeHelpers.GetDisplayNameOrDefault(prop),
                prop.Name.ToCamelCase(), prop.GetCustomAttributes<TableColumnAttribute>().FirstOrDefault())).ToList();
            if (!excludeActionsColumn)
            {
                list.Add(new TableColumn(await TranslationsRepository.GetValueOrSlug("actions"), "_actions", 100)
                    {Orderable = false, Searchable = false});
            }

            return list;
        }

        public virtual List<DetailsField> GetDetailsFields(Type viewModelType = null)
        {
            var props = (viewModelType ?? ViewModelType).GetProperties().ToList();
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
                .Select(prop =>
                {
                    var field = prop.GetCustomAttributes<DetailsFieldAttribute>().FirstOrDefault() is {} attr
                        ? attr.ToDetailsField()
                        : new DetailsField();
                    field.PropertyInfo = prop;
                    return field;
                }).OrderBy(df => df.OrderIndex).ToList();

            return list;
        }
    }
}