using AutoMapper;

namespace MCMS.Base.Data.MappingConfig
{
    public interface IMappingConfig
    {
        void CreateMaps(IMapperConfigurationExpression configExpression);
    }
}