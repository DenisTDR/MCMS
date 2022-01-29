using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Base.Helpers;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.TableConfig
{
    public class TableConfigService<TVm> : ITableConfigServiceT<TVm> where TVm : IViewModel
    {
        public TableConfigService(IUrlHelper urlHelper)
        {
            UrlHelper = urlHelper;
        }

        public virtual Type ViewModelType => typeof(TVm);
        public bool ExcludeActionsColumn { get; set; }
        public bool UseModals { get; set; }
        public object TableItemsApiUrlValues { get; set; }
        public bool UseCreateNewItemLink { get; set; } = true;
        public object CreateNewItemLinkValues { get; set; }
        public bool ExcludeDefaultItemActions { get; set; }

        public IUrlHelper UrlHelper { get; set; }


        public bool ServerSide { get; set; }

        public virtual string TableItemsApiUrl { get; set; }

        public virtual MRichLink CreateNewItemLink { get; set; }

        public virtual List<TableColumn> GetTableColumns()
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

            var list = tableColumnProps.Select(prop =>
                new TableColumn(prop, prop.GetCustomAttributes<TableColumnAttribute>().ToList())).ToList();
            if (!ExcludeActionsColumn)
            {
                list.Add(new TableColumn("<span class='col-name-hidden'>Actions</span>", "_actions", 100)
                    {Orderable = ServerClient.None, Searchable = ServerClient.None});
            }

            return list;
        }


        public virtual Task<TableConfig> GetTableConfig()
        {
            var config = new TableConfig
            {
                ModelName = TypeHelpers.GetDisplayNameOrDefault<TVm>(),
                TableColumns = GetTableColumns(),
                HasTableIndexColumn = true,
                TableItemsApiUrl = TableItemsApiUrl,
                ItemActions = GetItemActions(),
                BatchActions = GetBatchActions(),
                TableActions = GetTableActions(),
                ServerSide = ServerSide,
            };
            if (UseCreateNewItemLink)
            {
                config.CreateNewItemLink = CreateNewItemLink;
                if (UseModals)
                {
                    config.CreateNewItemLink.WithModal();
                }
            }

            if (AfterBuildHook != null) config = AfterBuildHook.Invoke(config);

            return Task.FromResult(config);
        }

        public Func<TableConfig, TableConfig> AfterBuildHook { get; set; }


        public virtual List<MRichLink> GetItemActions()
        {
            return new();
        }

        public virtual List<BatchAction> GetBatchActions(bool excludeDefault = false)
        {
            return new();
        }

        public virtual List<object> GetTableActions(bool excludeDefault = false)
        {
            if (excludeDefault)
            {
                return new();
            }

            return new()
            {
                "mcmsColVis",
                "pageLength"
            };
        }
    }
}