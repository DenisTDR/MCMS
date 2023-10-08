using System;
using System.Linq;
using System.Reflection;
using MCMS.Display.Link;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.ModelDisplay
{
    public class BatchAction : MRichLink
    {
        public string TitleAttr { get; set; }
        public BatchAction(string text, Type controller, MethodInfo action = null) : base(text, controller, action)
        {
        }
        public BatchAction(string text, Type controller, string actionName) : base(text, controller, actionName)
        {
        }
        public BatchAction(string text, string url) : base(text, url)
        {
        }

        public object GetConfigObject(IUrlHelper url)
        {
            this.SetData("url", BuildUrl(url));

            return new
            {
                text = this.BuildText(),
                name = Tag,
                data = AnchorData,
                className = CssClasses,
                extend = "selected",
                titleAttr = TitleAttr
            };
        }
    }
}