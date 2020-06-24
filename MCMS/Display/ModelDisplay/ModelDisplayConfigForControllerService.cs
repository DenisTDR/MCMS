using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay.Attributes;
using MCMS.Helpers;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public class
        ModelDisplayConfigForControllerService<TE, TFm, TVm, TUiController, TApiController> : IModelDisplayConfigService
        where TUiController : GenericUiController<TE, TFm, TVm, TApiController>
        where TE : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : IGenericApiController<TFm, TVm>
    {
        public ModelDisplayConfig GetTableConfig(IUrlHelper url, dynamic viewBag, bool createNewLink = true)
        {
            var config = new ModelDisplayConfig
            {
                IndexPageTitle = TypeHelpers.GetDisplayName<TUiController>(),
                ModelName = EntityHelper.GetEntityName<TE>(),
                TableColumns = GetTableColumns(),
                HasTableIndexColumn = true,
                TableItemsApiUrl = url.ActionLink(nameof(IReadOnlyApiController<TVm>.Index),
                    TypeHelpers.GetControllerName(typeof(TApiController)), viewBag.TableItemsApiUrlValues as object),
                TableItemActions = GetDefaultTableItemActions()
            };
            if (createNewLink)
            {
                config.CreateNewItemLink = new MRichLink(
                        "Create " + TypeHelpers.GetDisplayName(typeof(TE)),
                        typeof(TUiController), nameof(GenericUiController<TE, TFm, TVm, TApiController>.Create))
                    .AsButton("outline-primary")
                    .WithIconClasses("fas fa-plus").WithValues(viewBag.TableItemsApiUrlValues);
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
            var tableColumnProps = props.Where(prop => prop.GetCustomAttributes<TableColumnAttribute>().Any()).ToList();
            if (tableColumnProps.Count == 0)
            {
                tableColumnProps = props;
            }

            var list = tableColumnProps
                .Select(prop => new TableColumn(TypeHelpers.GetDisplayName(prop), prop.Name.ToCamelCase(),
                    prop.GetCustomAttributes<TableColumnAttribute>().FirstOrDefault()?.Order ?? 0)).ToList();
            if (!excludeActionsColumn)
            {
                list.Add(new TableColumn("Actions", "_actions", 100));
            }

            return list;
        }

        public virtual List<MRichLink> GetDefaultTableItemActions()
        {
            return new List<MRichLink>
            {
                new MRichLink("", typeof(TUiController),
                        nameof(GenericUiController<TE, TFm, TVm, TApiController>.Details))
                    .AsButton("outline-info").WithIconClasses("far fa-eye").WithValues(new {id = "ENTITY_ID"}),
                new MRichLink("", typeof(TUiController),
                        nameof(GenericUiController<TE, TFm, TVm, TApiController>.Edit))
                    .AsButton("outline-primary").WithModal().WithIconClasses("fas fa-pencil-alt"),
                new MRichLink("", typeof(TUiController),
                        nameof(GenericUiController<TE, TFm, TVm, TApiController>.Delete))
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