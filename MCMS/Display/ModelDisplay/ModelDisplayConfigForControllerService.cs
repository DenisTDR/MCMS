using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using MCMS.Controllers.Api;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public class
        ModelDisplayConfigForControllerService<TE, TFm, TVm, TUiController, TApiController> : ModelDisplayConfigService
        where TUiController : GenericAdminUiController<TE, TFm, TVm, TApiController>
        where TE : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : IGenericApiController<TFm, TVm>
    {
        public override Type ViewModelType => typeof(TVm);

        public override async Task<ModelDisplayTableConfig> GetTableConfig(IUrlHelper url, dynamic viewBag,
            bool createNewLink = true)
        {
            var config = new ModelDisplayTableConfig
            {
                IndexPageTitle = TypeHelpers.GetDisplayName<TUiController>(),
                ModelName = TypeHelpers.GetDisplayName<TVm>(),
                TableColumns = await GetTableColumns(),
                HasTableIndexColumn = true,
                TableItemsApiUrl = url.ActionLink(nameof(IReadOnlyApiController<TVm>.Index),
                    TypeHelpers.GetControllerName(typeof(TApiController)), viewBag.TableItemsApiUrlValues as object,
                    protocol: Utils.GetExternalProtocol()),
                TableItemActions = GetDefaultTableItemActions(viewBag)
            };
            if (createNewLink)
            {
                config.CreateNewItemLink = new MRichLink(
                        $"{await TranslationsRepository.GetValueOrSlug("create")} {TypeHelpers.GetDisplayName(typeof(TVm))}",
                        typeof(TUiController), nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Create))
                    .AsButton("outline-primary").WithIconClasses("fas fa-plus")
                    .WithValues(viewBag.CreateNewLinkValues as object);
                if (viewBag.UsesModals)
                {
                    config.CreateNewItemLink.WithModal();
                }
            }

            return config;
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

        public ModelDisplayConfigForControllerService(ITranslationsRepository translationsRepository) : base(
            translationsRepository)
        {
        }
    }
}