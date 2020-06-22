using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCMS.SwaggerFormly.Extensions
{
    public static class HtmlMSectionsExtensions
    {
        public static IDisposable BeginMPageScripts(this IHtmlHelper helper)
        {
            return helper.BeginMSection("page-scripts");
        }

        public static HtmlString RenderMPageScripts(this IHtmlHelper helper)
        {
            return helper.RenderMSection("page-scripts");
        }
        public static IDisposable BeginMScripts(this IHtmlHelper helper)
        {
            return helper.BeginMSection("scripts");
        }

        public static HtmlString RenderMScripts(this IHtmlHelper helper)
        {
            return helper.RenderMSection("scripts");
        }

        public static IDisposable BeginMStyles(this IHtmlHelper helper)
        {
            return helper.BeginMSection("styles");
        }

        public static HtmlString RenderMStyles(this IHtmlHelper helper)
        {
            return helper.RenderMSection("styles");
        }

        public static IDisposable BeginMSection(this IHtmlHelper helper, string sectionKey)
        {
            return new MSectionBlock(helper.ViewContext, sectionKey);
        }

        public static HtmlString RenderMSection(this IHtmlHelper helper, string sectionKey)
        {
            return new HtmlString(string.Join(Environment.NewLine,
                GetMSectionList(helper.ViewContext.HttpContext, sectionKey)));
        }

        private static List<string> GetMSectionList(HttpContext httpContext, string sectionKey)
        {
            sectionKey = "m-section-" + sectionKey;
            var mSectionList = (List<string>) httpContext.Items[sectionKey];
            if (mSectionList == null)
            {
                mSectionList = new List<string>();
                httpContext.Items[sectionKey] = mSectionList;
            }

            return mSectionList;
        }

        private class MSectionBlock : IDisposable
        {
            private readonly TextWriter _originalWriter;
            private readonly StringWriter _mSectionWriter;

            private readonly ViewContext _viewContext;
            private readonly string _sectionKey;

            public MSectionBlock(ViewContext viewContext, string sectionKey)
            {
                _viewContext = viewContext;
                _sectionKey = sectionKey;
                _originalWriter = _viewContext.Writer;
                _viewContext.Writer = _mSectionWriter = new StringWriter();
            }

            public void Dispose()
            {
                _viewContext.Writer = _originalWriter;
                var mSectionList = GetMSectionList(_viewContext.HttpContext, _sectionKey);
                mSectionList.Add(_mSectionWriter.ToString());
            }
        }
    }
}