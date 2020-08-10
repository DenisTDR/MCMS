using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace MCMS.Base.Display.DisplayValue
{
    public class DisplayValueService
    {
        private List<DisplayValueFormatters.TryDisplayDelegate> _formatters;
        public DisplayValueService(IOptions<DisplayValueFormatters> options)
        {
            _formatters = options.Value.Formatters;
        }

        public object GetDisplayValue(PropertyInfo propertyInfo, object obj)
        {
            foreach (var tryDisplayDelegate in _formatters)
            {
                if (tryDisplayDelegate(propertyInfo, obj, out var value))
                {
                    return value;
                }
            }

            return propertyInfo.GetValue(obj);
        }
    }
}