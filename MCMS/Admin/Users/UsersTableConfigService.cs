using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using MCMS.Display.Link;
using MCMS.Display.TableConfig;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Admin.Users
{
    public class UsersTableConfigService : TableConfigService<UserViewModel>
    {
        private readonly ITranslationsRepository _translationsRepository;

        public override Task<TableConfig> GetTableConfig()
        {
            TableItemsApiUrl ??= UrlHelper.ActionLink(ServerSide
                    ? nameof(AdminUsersAdminApiController.DtQuery)
                    : nameof(AdminUsersAdminApiController.Index),
                TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)), TableItemsApiUrlValues);
            CreateNewItemLink ??= new MRichLink(
                    $"{_translationsRepository.GetValueOrSlug("create").Result} {TypeHelpers.GetDisplayNameOrDefault(typeof(UserViewModel)).ToLowerFirstChar()}",
                    typeof(AdminUsersUiController), nameof(AdminUsersUiController.Create))
                .WithTag("create").AsButton("outline-primary").WithIconClasses("fas fa-plus")
                .WithValues(CreateNewItemLinkValues)
                .WithModal();


            return base.GetTableConfig();
        }

        public override List<MRichLink> GetItemActions()
        {
            return new()
            {
                new MRichLink("", typeof(AdminUsersUiController), nameof(AdminUsersUiController.Details))
                    .WithTag("details")
                    .AsButton("outline-info").WithModal().WithIconClasses("far fa-eye")
                    .WithValues(new { id = "ENTITY_ID" }),
                new MRichLink("", typeof(AdminUsersUiController), nameof(AdminUsersUiController.ChangeRoles))
                    .WithTag("roles").AsButton("outline-primary").WithModal().WithIconClasses("fas fa-tags")
                    .WithValues(new { id = "ENTITY_ID" }),
                new MRichLink("", typeof(AdminUsersUiController), nameof(AdminUsersUiController.Delete))
                    .WithTag("delete")
                    .AsButton("outline-danger").WithModal().WithIconClasses("fas fa-trash-alt")
                    .WithValues(new { id = "ENTITY_ID" })
            };
        }

        public UsersTableConfigService(ITranslationsRepository translationsRepository, IUrlHelper urlHelper) :
            base(urlHelper)
        {
            _translationsRepository = translationsRepository;
        }
    }
}