using System.Linq;

namespace MCMS.SwaggerFormly.Models
{
    public class FormlyFormParams
    {
        public string OpenApiConfigUrl { get; set; }
        public string SchemaName { get; set; }
        public FormActionType Action { get; set; }
        public string GetUrl { get; set; }
        public string SubmitUrl { get; set; }
        public object AdditionalFields { get; set; }
        public string ModelId { get; set; }
        public string FormInstanceId { get; set; }

        public string ToUrlQuery()
        {
            return string.Join("&", GetType().GetProperties()
                .Where(pi => !string.IsNullOrEmpty(pi.GetValue(this)?.ToString()))
                .Select(pi => pi.Name.First().ToString().ToLower() + pi.Name.Substring(1) + "=" + pi.GetValue(this)));
        }
    }

    public enum FormActionType
    {
        Create,
        Patch
    }
}