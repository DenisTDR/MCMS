using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MCMS.Base.Extensions
{
    public static class HtmlHelpersExtensions
    {
        public static async Task RenderBlindPartialAsync(this IHtmlHelper htmlHelper, string partialViewName,
            object model = null, ViewDataDictionary viewData = null)
        {
            await htmlHelper.RenderPartialAsync(partialViewName, model, viewData);
        }
        public static async Task<IHtmlContent> BlindPartialAsync(this IHtmlHelper htmlHelper, string partialViewName,
            object model = null, ViewDataDictionary viewData = null)
        {
            return await htmlHelper.PartialAsync(partialViewName, model, viewData);
        }
    }
}