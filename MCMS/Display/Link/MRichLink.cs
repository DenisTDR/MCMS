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

            return urlHelper.ActionLink(ActionName, ControllerName, Values);
        }

        public MRichLink AsButton(string cssClasses)
        {
            CssClasses = "btn btn-" + cssClasses;
            return this;
        }

        public MRichLink ToggleModal(bool isWith)
        {
            AsModal = isWith;
            return this;
        }

        public MRichLink WithModal(string backdrop = "static",
            ModalSuccessAction modalSuccessAction = ModalSuccessAction.ReloadTable)
        {
            ModalBackdrop = backdrop;
            AsModal = true;
            ModalSuccessAction = modalSuccessAction;
            return this;
        }

        public MRichLink WithValues(object values)
        {
            Values = values;
            return this;
        }
    }

    public enum ModalSuccessAction
    {
        [EnumMember(Value = null)] None,
        [EnumMember(Value = "reloadTable")] ReloadTable
    }
}