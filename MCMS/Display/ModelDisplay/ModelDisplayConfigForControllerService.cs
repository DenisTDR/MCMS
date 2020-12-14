using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using MCMS.Controllers.Api;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public class
        ModelDisplayConfigForControllerService<TE, TFm, TVm, TUiController, TApiController> : ModelDisplayConfigService,
            IModelDisplayConfigForControllerService
        where TUiController : GenericAdminUiController<TE, TFm, TVm, TApiController>
        where TE : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : ICrudAdminApiController<TFm, TVm>
    {
        public override Type ViewModelType => typeof(TVm);
     
        public override async Task<IndexPageConfig> GetIndexPageConfig(IUrlHelper url)
        {
            var config = new IndexPageConfig
            {
                IndexPageTitle = TypeHelpers.GetDisplayNameOrDefault<TUiController>(),
                TableConfig = await GetTableConfig(url)
            };
            return config;
        }

        public override async Task<TableDisplayConfig> GetTableConfig(IUrlHelper url)
        {
            var config = new TableDisplayConfig
            {
                ModelName = TypeHelpers.GetDisplayNameOrDefault<TVm>(),
                TableColumns = await GetTableColumns(),
                HasTableIndexColumn = true,
                TableItemsApiUrl = url.ActionLink(nameof(IReadOnlyApiController<TVm>.Index),
                    TypeHelpers.GetControllerName(typeof(TApiController)), TableItemsApiUrlValues),
                ItemActions = GetItemActions(),
                BatchActions = GetBatchActions(),
                TableActions = GetTableActions()
            };
            if (UseCreateNewItemLink)
            {
                config.CreateNewItemLink = new MRichLink(
                        $"{await TranslationsRepository.GetValueOrSlug("create")} {TypeHelpers.GetDisplayNameOrDefault(typeof(TVm)).ToLowerFirstChar()}",
                        typeof(TUiController), nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Create))
                    .WithTag("create").AsButton("outline-primary").WithIconClasses("fas fa-plus")
                    .WithValues(CreateNewItemLinkValues);
                if (UseModals)
                {
                    config.CreateNewItemLink.WithModal();
                }
            }

            return config;
        }


        public virtual List<MRichLink> GetItemActions()
        {
            if (ExcludeDefaultItemActions)
            {
                return new List<MRichLink>();
            }

            return new List<MRichLink>
            {
                new MRichLink("", typeof(TUiController),
                        nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Details)).WithTag("details")
                    .AsButton("outline-info").WithModal().ToggleModal(UseModals)
                    .WithIconClasses("far fa-eye").WithValues(new {id = "ENTITY_ID"}),
                new MRichLink("", typeof(TUiController),
                        nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Edit)).WithTag("edit")
                    .AsButton("outline-primary").WithModal().ToggleModal(UseModals)
                    .WithIconClasses("fas fa-pencil-alt"),
                new MRichLink("", typeof(TUiController),
                        nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Delete)).WithTag("delete")
                    .AsButton("outline-danger").WithModal().WithIconClasses("fas fa-trash-alt")
            }.Select(l => l.WithValues(new {id = "ENTITY_ID"})).ToList();
        }

        public virtual List<BatchAction> GetBatchActions(bool excludeDefault = false)
        {
            if (excludeDefault)
            {
                return new();
            }

            return new()
            {
                new BatchAction("", typeof(TUiController),
                            nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.BatchDelete))
                        {TitleAttr = "Delete selected items"}
                    .WithTag("batch-delete").WithIconClasses("fas fa-trash").AsButton("outline-danger btn-light")
                    .WithModal()
            };
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

        public static Type MakeGenericTypeWithUiControllerType(Type uiControllerType)
        {
            return typeof(ModelDisplayConfigForControllerService<,,,,>).MakeGenericType(typeof(TE), typeof(TFm),
                typeof(TVm), uiControllerType, typeof(TApiController));
        }

        public ModelDisplayConfigForControllerService(ITranslationsRepository translationsRepository) : base(
            translationsRepository)
        {
        }
    }
}