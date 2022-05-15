using AutoMapper;
using MCMS.Base.Auth;
using MCMS.Base.Data.MappingConfig;

namespace MCMS.Admin.Users
{
    public class UserMapper : EntityViewModelMappingConfig<User, UserViewModel>
    {
        protected override void ConfigureModelToEntityMap(IMappingExpression<UserViewModel, User> mappingExpression)
        {
            mappingExpression.ForMember(u => u.RolesList, cfg => cfg.Ignore());
        }
    }
}