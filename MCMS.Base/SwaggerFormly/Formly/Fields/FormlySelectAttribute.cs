using Microsoft.OpenApi.Any;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlySelectAttribute : FormlyFieldAttribute
    {
        public string OptionsUrl { get; }
        public string LabelProp { get; }
        public string ValueProp { get; }

        public FormlySelectAttribute(string optionsUrl, string labelProp = "name", string valueProp = "id") 
        {
            OptionsUrl = optionsUrl;
            LabelProp = labelProp;
            ValueProp = valueProp;
            Type = "select";
        }

        public override OpenApiObject GetOpenApiConfig()
        {
            var obj = new OpenApiObject
            {
                ["optionsUrl"] = new OpenApiString(OptionsUrl),
                ["labelProp"] = new OpenApiString(LabelProp),
                ["valueProp"] = new OpenApiString(ValueProp)
            };

            return obj;
        }
    }
}