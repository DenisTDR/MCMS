using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Api;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public class
        ModelDisplayConfigForControllerService<TE, TFm, TVm, TUiController, TApiController> : IModelDisplayConfigService
        where TUiController : GenericAdminUiController<TE, TFm, TVm, TApiController>
        where TE : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : IGenericApiController<TFm, TVm>
    {
        public ModelDisplayTableConfig GetTableConfig(IUrlHelper url, dynamic viewBag, bool createNewLink = true)
        {
            var config = new ModelDisplayTableConfig
            {
                IndexPageTitle = TypeHelpers.GetDisplayName<TUiController>(),
                ModelName = EntityHelper.GetEntityName<TE>(),
                TableColumns = GetTableColumns(),
                HasTableIndexColumn = true,
                TableItemsApiUrl = url.ActionLink(nameof(IReadOnlyApiController<TVm>.Index),
                    TypeHelpers.GetControllerName(typeof(TApiController)), viewBag.TableItemsApiUrlValues as object),
                TableItemActions = GetDefaultTableItemActions(viewBag)
            };
            if (createNewLink)
            {
                config.CreateNewItemLink = new MRichLink(
                        "Create " + TypeHelpers.GetDisplayName(typeof(TE)),
                        typeof(TUiController), nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Create))
                    .AsButton("outline-primary")
                    .WithIconClasses("fas fa-plus").WithValues(viewBag.CreateNewLinkValues);
                if (viewBag.UsesModals)
                {
                    config.CreateNewItemLink.WithModal();
                }
            }

            return config;
        }

        public virtual List<TableColumn> GetTableColumns(bool excludeActionsColumn = false)
        {
            var props = typeof(TVm).GetProperties().ToList();
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

        public virtual List<MRichLink> GetDefaultTableItemActions(dynamic viewBag)
        {
            return new List<MRichLink>
            {
                new MRichLink("", typeof(TUiController),
                        nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Details)).WitTag("details")
                    .AsButton("outline-info").WithModal().ToggleModal((bool) viewBag.UsesModals)
                    .WithIconClasses("far fa-eye").WithValues(new {id = "ENTITY_ID"}),
                new MRichLink("", typeof(TUiController),
                        nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Edit)).WitTag("edit")
                    .AsButton("outline-primary").WithModal().ToggleModal((bool) viewBag.UsesModals)
                    .WithIconClasses("fas fa-pencil-alt"),
                new MRichLink("", typeof(TUiController),
                        nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Delete)).WitTag("delete")
                    .AsButton("outline-danger").WithModal().WithIconClasses("fas fa-trash-alt")
            };
        }

        public static Type MakeGenericTypeWithUiControllerType(Type uiControllerType)
        {
            return typeof(ModelDisplayConfigForControllerService<,,,,>).MakeGenericType(typeof(TE), typeof(TFm),
                typeof(TVm), uiControllerType, typeof(TApiController));
        }
    }
}