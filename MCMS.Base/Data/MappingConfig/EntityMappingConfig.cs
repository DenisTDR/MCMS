using AutoMapper;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;

namespace MCMS.Base.Data.MappingConfig
{
    public class EntityMappingConfig<TE, TVm> : IMappingConfig where TE : Entity where TVm : class, IViewModel
    {
        public virtual void CreateMaps(IMapperConfigurationExpression configExpression)
        {
            ConfigureEntityToViewModelMap(configExpression.CreateMap<TE, TVm>());
            ConfigureViewModelToEntityMap(configExpression.CreateMap<TVm, TE>());
        }

        protected virtual void ConfigureViewModelToEntityMap(IMappingExpression<TVm, TE> mappingExpression)
        {
        }

        protected virtual void ConfigureEntityToViewModelMap(IMappingExpression<TE, TVm> mappingExpression)
        {
        }
    }
}