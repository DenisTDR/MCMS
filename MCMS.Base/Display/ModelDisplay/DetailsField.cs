using System.Reflection;
using MCMS.Base.Display.DisplayValue;
using MCMS.Base.Helpers;

namespace MCMS.Base.Display.ModelDisplay
{
    public class DetailsField
    {
        public double OrderIndex { get; set; }
        public string Tag { get; set; }
        public string Name => TypeHelpers.GetDisplayName(PropertyInfo);
        public string Description => TypeHelpers.GetDescription(PropertyInfo);
        public PropertyInfo PropertyInfo { get; set; }
        public string ClassName { get; set; }

        public object GetDisplayValue(DisplayValueService displayValueService, object obj)
        {
            return displayValueService.GetDisplayValue(PropertyInfo, obj);
        }
    }
}