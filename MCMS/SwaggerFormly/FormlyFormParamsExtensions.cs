using System.Collections.Generic;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly
{
    public static class FormlyFormParamsExtensions
    {
        public static void AddOption(this FormlyFormParams th, string key, object value)
        {
            th.Options ??= new Dictionary<string, object>();
            th.Options[key] = value;
        }

        public static void AddFormStateValue(this FormlyFormParams th, string key, object value)
        {
            th.Options ??= new Dictionary<string, object>();

            if (!th.Options.ContainsKey("formState") || !(th.Options["formState"] is Dictionary<string, object> dict1))
            {
                dict1 = new Dictionary<string, object>();
                th.Options["formState"] = dict1;
            }

            dict1[key] = value;
        }
    }
}