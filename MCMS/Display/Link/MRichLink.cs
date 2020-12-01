using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.Link
{
    public class MRichLink : MLink
    {
        public bool AsModal { get; set; }
        public string CssClasses { get; set; }
        public string ModalBackdrop { get; set; }
        public object Values { get; set; }
        public ModalSuccessAction ModalSuccessAction { get; set; }
        public string ModalSuccessCallback { get; set; }


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

            return urlHelper.ActionLink(ActionName, ControllerName, Values);
        }
    }

    public enum ModalSuccessAction
    {
        [EnumMember(Value = null)] None,
        [EnumMember(Value = "runCallback")] RunCallback
    }
}