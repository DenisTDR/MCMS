using System;

namespace MCMS.Base.SwaggerFormly.Formly
{
    public class ReplacesPathAttribute : Attribute
    {
        public ReplacesPathAttribute(string pathToReplace)
        {
            PathToReplace = pathToReplace;
        }

        public string PathToReplace { get; set; }
    }
}