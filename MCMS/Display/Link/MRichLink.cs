using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.Link
{
    public class MRichLink : MLink
    {
        private string _tag;

        public override string Tag
        {
            get => _tag;
            set
            {
                this.SetData("tag", value);
                _tag = value;
            }
        }

        public string CssClasses { get; set; }

        public object Values { get; set; }

        public string ModalSuccessCallback
        {
            get => this.GetData("modal-callback") as string;
            set => this.SetData("modal-callback", value);
        }

        public Dictionary<string, object> AnchorData { get; set; }


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
                return urlHelper?.Content(Url);
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