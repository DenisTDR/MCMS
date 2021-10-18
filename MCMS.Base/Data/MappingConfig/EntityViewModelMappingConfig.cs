using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;

namespace MCMS.Base.Data.MappingConfig
{
    public class EntityViewModelMappingConfig<TE, TVm> : EntityModelMappingConfig<TE, TVm>
        where TE : IEntity
        where TVm : IViewModel
    {
    }
}