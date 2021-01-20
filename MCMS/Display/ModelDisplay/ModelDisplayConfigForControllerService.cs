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
using MCMS.Display.TableConfig;
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
        private readonly ITranslationsRepository _translationsRepository;
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

        public override Task<TableConfig.TableConfig> GetTableConfig(IUrlHelper url)
        {
            throw new NotImplementedException();
        }

        public List<MRichLink> GetItemActions()
        {
            throw new NotImplementedException();
        }

        public static Type MakeGenericTypeWithUiControllerType(Type uiControllerType)
        {
            return typeof(ModelDisplayConfigForControllerService<,,,,>).MakeGenericType(typeof(TE), typeof(TFm),
                typeof(TVm),
                uiControllerType, typeof(TApiController));
        }

        public ModelDisplayConfigForControllerService(
            ITranslationsRepository translationsRepository)
        {
            _translationsRepository = translationsRepository;
        }
    }
}