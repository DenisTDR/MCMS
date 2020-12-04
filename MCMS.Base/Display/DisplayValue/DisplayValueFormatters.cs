using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MCMS.Base.Display.DisplayValue
{
    public class DisplayValueFormatters
    {
        public delegate bool TryDisplayDelegate(PropertyInfo pInfo, object obj, out object value);
        
        private readonly List<TryDisplayDelegate> _formatters = new();
        public List<TryDisplayDelegate> Formatters => _formatters.ToList();

        public void Add(TryDisplayDelegate formatter)
        {
            _formatters.Add(formatter);
        }
    }
}