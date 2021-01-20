using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using MCMS.Display.Link;
using MCMS.Display.TableConfig;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Admin.Users
{
    public class UsersTableModelDisplayConfigService : TableConfigService<UserViewModel>
    {
        // public override async Task<TableConfig> GetTableConfig()
        // {
        //     return new()
        //     {
        //         ModelName = "User",
        //         TableColumns = null,
        //         HasTableIndexColumn = true,
        //         TableItemsApiUrl = UrlHelper.ActionLink(nameof(AdminUsersAdminApiController.Index),
        //             TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController))),
        //         ItemActions = GetDefaultTableItemActions()
        //     };
        // }

        public override Task<TableConfig> GetTableConfig()
        {
            if (TableItemsApiUrl == null)
            {
                TableItemsApiUrl = UrlHelper.ActionLink(ServerSide
                        ? nameof(AdminUsersAdminApiController.DtQuery)
                        : nameof(AdminUsersAdminApiController.Index),
                    TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)), TableItemsApiUrlValues);
            }

            return base.GetTableConfig();
        }

        public override List<MRichLink> GetItemActions()
        {
            return new()
            {
                new MRichLink("", typeof(AdminUsersController), nameof(AdminUsersController.Details)).WithTag("details")
                    .AsButton("outline-info").WithModal().WithIconClasses("far fa-eye")
                    .WithValues(new {id = "ENTITY_ID"}),
                new MRichLink("", typeof(AdminUsersController), nameof(AdminUsersController.ChangeRoles))
                    .WithTag("roles").AsButton("outline-primary").WithModal().WithIconClasses("fas fa-tags")
                    .WithValues(new {id = "ENTITY_ID"}),
                new MRichLink("", typeof(AdminUsersController), nameof(AdminUsersController.Delete)).WithTag("delete")
                    .AsButton("outline-danger").WithModal().WithIconClasses("fas fa-trash-alt")
                    .WithValues(new {id = "ENTITY_ID"})
            };
        }

        public UsersTableModelDisplayConfigService(ITranslationsRepository translationsRepository,
            IUrlHelper urlHelper)
            : base(urlHelper)
        {
        }
    }
}