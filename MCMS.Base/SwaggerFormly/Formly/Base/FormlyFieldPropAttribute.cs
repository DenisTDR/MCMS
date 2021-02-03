using System.IO;

namespace MCMS.Base.SwaggerFormly.Formly.Base
{
    public class FormlyFieldPropAttribute : BasicAttribute
    {
        public string ObjectPath { get; set; }
        public string FullPath => Path.Combine(ObjectPath ?? "", Name).Replace("\\", "/");

        public FormlyFieldPropAttribute(string name, object value, string objectPath = null,
            bool onTemplateOptions = false) : base(name, value)
        {
            objectPath ??= "";
            ObjectPath = onTemplateOptions ? $"templateOptions/{objectPath}" : objectPath;
        }

        public FormlyFieldPropAttribute(string name, object value, bool onTemplateOptions) : this(name, value,
            onTemplateOptions ? "templateOptions" : "")
        {
        }
    }
}