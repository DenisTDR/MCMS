using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Display.ModelDisplay;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public abstract class ModelDisplayConfigService : IModelDisplayConfigService
    {
        // protected ITranslationsRepository TranslationsRepository { get; }

        public abstract Type ViewModelType { get; }

        public object TableItemsApiUrlValues { get; set; }
        public bool UseCreateNewItemLink { get; set; } = true;
        public object CreateNewItemLinkValues { get; set; }
        public bool UseModals { get; set; }
        public bool ExcludeDefaultItemActions { get; set; }

        public abstract Task<IndexPageConfig> GetIndexPageConfig(IUrlHelper url);

        public abstract Task<TableConfig.TableConfig> GetTableConfig(IUrlHelper url);
        public List<DetailsField> GetDetailsFields(Type viewModelType = null)
        {
            throw new NotImplementedException();
        }
    }
}