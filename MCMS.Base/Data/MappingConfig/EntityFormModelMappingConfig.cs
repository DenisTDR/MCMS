using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;

namespace MCMS.Base.Data.MappingConfig
{
    public class EntityFormModelMappingConfig<TE, TFm> : EntityModelMappingConfig<TE, TFm>
        where TE : IEntity
        where TFm : IFormModel
    {
    }
}