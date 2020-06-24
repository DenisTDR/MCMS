using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;

namespace MCMS.Base.Data.MappingConfig
{
    public class EntityViewModelMappingConfig<TE, TFm> : EntityModelMappingConfig<TE, TFm>
        where TE : IEntity
        where TFm : IViewModel
    {
    }
}