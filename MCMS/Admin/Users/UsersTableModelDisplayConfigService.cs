using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using MCMS.Display.Link;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Admin.Users
{
    public class UsersTableModelDisplayConfigService : ModelDisplayConfigService
    {
        public override Type ViewModelType => typeof(UserViewModel);

        public override async Task<ModelDisplayTableConfig> GetTableConfig(IUrlHelper url, dynamic viewBag,
            bool createNewLink = true)
        {
            var config = new ModelDisplayTableConfig
            {
                IndexPageTitle = "Users",
                ModelName = "User",
                TableColumns = await GetTableColumns(),
                HasTableIndexColumn = true,
                TableItemsApiUrl = url.ActionLink(nameof(AdminUsersAdminApiController.Index),
                    TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)),
                    viewBag.TableItemsApiUrlValues as object, protocol: Utils.GetExternalProtocol()),
                TableItemActions = GetDefaultTableItemActions(viewBag)
            };
            return config;
        }

        public virtual List<MRichLink> GetDefaultTableItemActions(dynamic viewBag)
        {
            return new List<MRichLink>
            {
                new MRichLink("", typeof(AdminUsersController), nameof(AdminUsersController.Details)).WitTag("details")
                    .AsButton("outline-info").WithModal().WithIconClasses("far fa-eye")
                    .WithValues(new {id = "ENTITY_ID"}),
                new MRichLink("", typeof(AdminUsersController), nameof(AdminUsersController.ChangeRoles))
                    .WitTag("roles").AsButton("outline-primary").WithModal().WithIconClasses("fas fa-tags")
                    .WithValues(new {id = "ENTITY_ID"}),
                new MRichLink("", typeof(AdminUsersController), nameof(AdminUsersController.Delete)).WitTag("delete")
                    .AsButton("outline-danger").WithModal().WithIconClasses("fas fa-trash-alt")
                    .WithValues(new {id = "ENTITY_ID"})
            };
        }

        public UsersTableModelDisplayConfigService(ITranslationsRepository translationsRepository) : base(
            translationsRepository)
        {
        }
    }
}