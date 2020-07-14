using AutoMapper;
using MCMS.Base.Data.MappingConfig;

namespace MCMS.Common.Translations.Translations.Item
{
    public class TranslationItemMappingConfig : IMappingConfig
    {
        public void CreateMaps(IMapperConfigurationExpression configExpression)
        {
            configExpression.CreateMap<TranslationItemFormModel, ItemForTranslationFormModel>();
            configExpression.CreateMap<ItemForTranslationFormModel, TranslationItemFormModel>();
            configExpression.CreateMap<ItemForTranslationFormModel, TranslationItemEntity>();
            configExpression.CreateMap<TranslationItemEntity, ItemForTranslationFormModel>();
        }
    }
}