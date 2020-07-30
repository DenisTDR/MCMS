using System;
using System.Reflection;
using System.Runtime.Serialization;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.Link
{
    public class MRichLink : MLink
    {
        public bool AsModal { get; set; }
        public string CssClasses { get; set; }
        public string ModalBackdrop { get; set; }
        public object Values { get; set; } = new {id = "ENTITY_ID"};
        public ModalSuccessAction ModalSuccessAction { get; set; }


        public MRichLink(string text, Type controller, MethodInfo action = null) : base(text, controller,
            action)
        {
        }

        public MRichLink(string text, Type controller, string actionName) : base(text, controller, actionName)
        {
        }

        public MRichLink(string text, string url) : base(text, url)
        {
        }

        public override string BuildUrl(IUrlHelper urlHelper = null)
        {
            if (urlHelper == null || Controller == null)
            {
                return Url;
            }

            return urlHelper.ActionLink(ActionName, ControllerName, Values, protocol: Utils.GetExternalProtocol());
        }

    }

    public enum ModalSuccessAction
    {
        [EnumMember(Value = null)] None,
        [EnumMember(Value = "reloadTable")] ReloadTable
    }
}