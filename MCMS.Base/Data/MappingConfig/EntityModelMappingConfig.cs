using AutoMapper;

namespace MCMS.Base.Data.MappingConfig
{
    public class EntityModelMappingConfig<TE, TM> : IMappingConfig
    {
        public virtual void CreateMaps(IMapperConfigurationExpression configExpression)
        {
            ConfigureEntityToModelMap(configExpression.CreateMap<TE, TM>());
            ConfigureModelToEntityMap(configExpression.CreateMap<TM, TE>());
        }

        protected virtual void ConfigureModelToEntityMap(IMappingExpression<TM, TE> mappingExpression)
        {
        }

        protected virtual void ConfigureEntityToModelMap(IMappingExpression<TE, TM> mappingExpression)
        {
        }
    }
}