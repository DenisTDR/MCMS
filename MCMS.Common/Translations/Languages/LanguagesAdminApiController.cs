using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Common.Translations.Languages
{
    [Authorize(Roles = "Admin")]
    public class LanguagesAdminApiController : CrudAdminApiController<LanguageEntity, LanguageFormModel, LanguageViewModel>
    {
    }
}