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
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.TableConfig
{
    public class TableConfigForControllerService<TE, TFm, TVm, TUiController, TApiController> : TableConfigService<TVm>
        where TUiController : GenericAdminUiController<TE, TFm, TVm, TApiController>
        where TE : class, IEntity
        where TFm : class, IFormModel
        where TVm : class, IViewModel
        where TApiController : ICrudAdminApiController<TFm, TVm>
    {
        private readonly ITranslationsRepository _translationsRepository;

        public TableConfigForControllerService(IUrlHelper urlHelper, ITranslationsRepository translationsRepository) :
            base(urlHelper)
        {
            _translationsRepository = translationsRepository;
        }

        public override List<MRichLink> GetItemActions()
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

        public override List<BatchAction> GetBatchActions(bool excludeDefault = false)
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

        public override Task<TableConfig> GetTableConfig()
        {
            if (CreateNewItemLink == null)
            {
                CreateNewItemLink = new MRichLink(
                        $"{_translationsRepository.GetValueOrSlug("create").Result} {TypeHelpers.GetDisplayNameOrDefault(typeof(TVm)).ToLowerFirstChar()}",
                        typeof(TUiController), nameof(GenericAdminUiController<TE, TFm, TVm, TApiController>.Create))
                    .WithTag("create").AsButton("outline-primary").WithIconClasses("fas fa-plus")
                    .WithValues(CreateNewItemLinkValues);
            }

            if (TableItemsApiUrl == null)
            {
                TableItemsApiUrl = UrlHelper.ActionLink(ServerSide
                        ? nameof(IReadOnlyApiController<TVm>.DtQuery)
                        : nameof(IReadOnlyApiController<TVm>.Index),
                    TypeHelpers.GetControllerName(typeof(TApiController)), TableItemsApiUrlValues);
            }

            return base.GetTableConfig();
        }

        public static Type MakeGenericTypeWithUiControllerType(Type uiControllerType)
        {
            return typeof(TableConfigForControllerService<,,,,>).MakeGenericType(typeof(TE), typeof(TFm), typeof(TVm),
                uiControllerType, typeof(TApiController));
        }
    }
}